using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private const int RADIUS_GROUNDING_RAY_COUNT = 8;
    private const float HEAD_COLLISION_GAP = .01f; // The amount of space to leave between upper capsule collision and object above when transitioning from crouching to standing.

    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private PlayerInput playerInput = null;
    [SerializeField] private LayerMask groundedLayerMask;
    [SerializeField] float groundedCheckDistance = 0.2f;
    [Tooltip("Height distance from top of collision capsule.")]
    [SerializeField] private float fpsCameraHeight = -0.2f;

    [Header("Speed")]
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float sprintSpeedMultiplier = 3f;
    [SerializeField] private float crouchingSpeedMultiplier = 0.3f;
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private float terminalVelocity = -60;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5; //Temp, probably replace with press and hold input?
    [SerializeField] private float crouchingJumpForce = 2.5f;

    [Header("Standing/Crouching")]
    [SerializeField] private float crouchTransitionSpeed = 4f;
    [SerializeField] private float standingHeight = 1.7f;
    [SerializeField] private float crouchingHeight = 0.7f;
    [SerializeField] private float standingStepOffset = 0.3f;
    [SerializeField] private float crouchingStepOffset = 0.1f;

    [SerializeField] Transform visualTransform = null;

    bool isGrounded = false;
    bool isJumping = false;

    private float yVelocity = 0;
    private Vector3 movementDampVelocity = Vector3.zero;
    private Vector3 motion;
    private bool crouchToggledOn = false;

    /// <summary>
    /// Calculated when character height is changed: 0 = full crouch | 1 = full stand
    /// </summary>
    private float crouchStandMovementFactor; 

    /// <summary>
    /// Calculated movement speed based on crouching and sprinting state.
    /// </summary>
    private float CalculatedMovementSpeed
    {
        get
        {
            float speed = walkSpeed * Mathf.Lerp(crouchingSpeedMultiplier, 1, crouchStandMovementFactor);

            if (playerInput.GetSprintHeld())
                speed *= sprintSpeedMultiplier;

            return speed;
        }
    }

    /// <summary>
    /// Returns true if character is in a crouching state.
    /// </summary>
    public bool IsCrouching => crouchStandMovementFactor < .5f;

    Transform connectedMovingPlatform;
    Vector3 connectedMovingPlatformPreviousPosition;

    // Start is called before the first frame update
    void Start()
    {
        if (crouchingHeight < characterController.radius * 2f) //radius * 2 is minimum height for a capsule
            crouchingHeight = characterController.radius * 2f;

        SetCharacterToStandingHeight();
    }

    [ContextMenu("Set Character to the Standing Height")]
    private void SetCharacterToStandingHeight()
    {
        SetCharacterControllerHeight(standingHeight);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 groundedNormal = Vector3.zero;
        if(isGrounded || yVelocity <= 0) //If we're in the air, and headed upwards, we dont check grounded
            isGrounded = CheckGrounding(out groundedNormal);

        if (isGrounded)
        {
            yVelocity = 0;
            if(isJumping)//We hit the ground, exit the jump
                isJumping = false;
            else
            {
                if(playerInput.GetJumpRequested())
                {
                    isJumping = true;
                    yVelocity = Mathf.Lerp(crouchingJumpForce, jumpForce, crouchStandMovementFactor);//Temp simple jump
                    isGrounded = false;
                }
            }
        }
        else
        {//Apply gravity to stop our upward, and allow it to go negative, but not more than terminal velocity!
            yVelocity += Physics.gravity.y * Time.deltaTime;
            yVelocity = Mathf.Max(yVelocity, terminalVelocity);
        }

        Vector3 moveInput = playerInput.GetMovementInput();
        if (moveInput != Vector3.zero)
        {
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;

            forward.y = 0;
            right.y = 0;
            forward = forward.normalized;
            right = right.normalized;

            Vector3 forwardRelativeMovement = moveInput.z * forward;
            Vector3 rightRelativeMovement = moveInput.x * right;

            Vector3 cameraRelativeMovement = forwardRelativeMovement + rightRelativeMovement;
            moveInput = cameraRelativeMovement;
        }

        if (playerInput.GetToggleCrouchPressed())
            crouchToggledOn = !crouchToggledOn;

        bool crouchingThisFrame = crouchToggledOn;

        if (playerInput.GetCrouchHeld())
        {
            crouchToggledOn = false; // overrides toggle functionality
            crouchingThisFrame = true;
        }

        if (crouchingThisFrame == true)
            CrouchDown();
        else
            StandUp();

        if(isGrounded)
        {
            moveInput = Vector3.ProjectOnPlane(moveInput, groundedNormal);
            moveInput.Normalize();
        }
        motion = Vector3.SmoothDamp(motion, moveInput * CalculatedMovementSpeed, ref movementDampVelocity, accelerationTime);

        Vector3 motionToUse = motion + Vector3.up * yVelocity; //Apply our gravity seperately from the projected horizontal movement

        Vector3 movingGroundMotion = Vector3.zero;

        if (connectedMovingPlatform)
        {
            movingGroundMotion = connectedMovingPlatform.position - connectedMovingPlatformPreviousPosition;
            connectedMovingPlatformPreviousPosition = connectedMovingPlatform.position;
        }

        characterController.Move((motionToUse * Time.deltaTime) + (movingGroundMotion));
    }

    /// <summary>
    /// Shrinks the Character Contoller until the it reaches the crouching size
    /// </summary>
    private void CrouchDown()
    {
        if (Mathf.Approximately(crouchingHeight, characterController.height) == false)
        {
            float targetHeight = Mathf.Max(crouchingHeight, characterController.height - (crouchTransitionSpeed * Time.deltaTime));
            SetCharacterControllerHeight(targetHeight);
        }
    }

    /// <summary>
    /// Grows the  Character Contoller until it reaches the standing size
    /// </summary>
    private void StandUp()
    {
        if (Mathf.Approximately(standingHeight, characterController.height) == false)
        {
            float targetHeight = Mathf.Min(standingHeight, characterController.height + (crouchTransitionSpeed * Time.deltaTime));

            Vector3 currentCenterPosition = transform.TransformPoint(characterController.center);
            Vector3 currentHeightPosition = currentCenterPosition + (Vector3.up * ((characterController.height / 2f) - characterController.radius));
            float targetHeightChange = targetHeight - characterController.height;

            if (Physics.SphereCast(currentHeightPosition, characterController.radius, Vector3.up, out RaycastHit hit, targetHeightChange, groundedLayerMask))
            {
                targetHeight = characterController.height + (hit.distance - HEAD_COLLISION_GAP); //head collision
            }

            SetCharacterControllerHeight(targetHeight);
        }
    }

    /// <summary>
    /// Sets a the Character Controller height. Also sets FPS camera position and Character Controller center position, & Character Controller step offset accordingly. 
    /// </summary>
    private void SetCharacterControllerHeight(float targetHeight)
    {
        crouchStandMovementFactor = Mathf.InverseLerp(crouchingHeight, standingHeight, targetHeight);
        if(visualTransform)
        {//Scale the visuals to help visualize
            float heightScale = Mathf.Lerp(crouchingHeight / standingHeight, 1, crouchStandMovementFactor);
            visualTransform.transform.localScale = new Vector3(visualTransform.transform.localScale.x, heightScale, visualTransform.transform.localScale.z);
        }
        float centerHeight = targetHeight / 2f;
        characterController.height = targetHeight;
        characterController.center = new Vector3(0, centerHeight, 0);
        PlayerCore.LocalPlayer.FpsCameraRootTransform.localPosition = new Vector3(0, targetHeight + fpsCameraHeight, 0);
        characterController.stepOffset = Mathf.Lerp(crouchingStepOffset, standingStepOffset, crouchStandMovementFactor);
    }

    private bool CheckGrounding(out Vector3 averageNormal)
    {
        averageNormal = Vector3.zero;
        Vector3 allRaysVerticalOffset = characterController.center + Vector3.up * (groundedCheckDistance - 0.01f - characterController.height / 2f);

        //Center grounding ray check
        Ray centerGroundedRay = new Ray(transform.position + allRaysVerticalOffset, Vector3.down);
        Debug.DrawLine(centerGroundedRay.origin, centerGroundedRay.origin + centerGroundedRay.direction * groundedCheckDistance, Color.red);

        RaycastHit hitInfo;
        bool hitGround = false;
        if (Physics.Raycast(centerGroundedRay, out hitInfo, groundedCheckDistance, groundedLayerMask, QueryTriggerInteraction.Ignore))
        {
            averageNormal += hitInfo.normal;
            hitGround = true;
        }

        //Radius grounding rays check
        for (int i = 0; i < RADIUS_GROUNDING_RAY_COUNT; i++)
        {
            //Calculate the XZ offsets for each of the 8 points in a circle: 
            float angleRadian = i * Mathf.PI * 2f / (float)RADIUS_GROUNDING_RAY_COUNT; //Get the radian value of each angle (0-2π) 
            Vector3 radiusRayOffset = new Vector3(Mathf.Cos(angleRadian) * characterController.radius, 0, Mathf.Sin(angleRadian) * characterController.radius); //Get point on the circumference at that angle

            Ray radiusRay = new Ray(transform.position + radiusRayOffset + allRaysVerticalOffset, Vector3.down);
            Debug.DrawLine(radiusRay.origin, radiusRay.origin + radiusRay.direction * groundedCheckDistance, Color.red);

            if (Physics.Raycast(radiusRay, out hitInfo, groundedCheckDistance, groundedLayerMask, QueryTriggerInteraction.Ignore))
            {
                averageNormal += hitInfo.normal;
                hitGround = true;
            }
        }

        if(hitGround)
            averageNormal.Normalize();
        return hitGround;
    }

    public void SetMovingSurface(Transform movingSurfaceTransform)
    {
        connectedMovingPlatform = movingSurfaceTransform;
        connectedMovingPlatformPreviousPosition = movingSurfaceTransform.position;
    }

    public void UnsetMovingSurface(Transform movingSurfaceTransform)
    {
        if(connectedMovingPlatform == movingSurfaceTransform)
            connectedMovingPlatform = null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 allRaysVerticalOffset = characterController.center + Vector3.up * (groundedCheckDistance - 0.01f - characterController.height / 2f);
        
        //Center grounding ray check
        Ray centerGroundedRay = new Ray(transform.position + allRaysVerticalOffset, Vector3.down);
        Debug.DrawLine(centerGroundedRay.origin, centerGroundedRay.origin + centerGroundedRay.direction * groundedCheckDistance, Color.red);

        //Radius grounding rays check
        for(int i = 0; i < RADIUS_GROUNDING_RAY_COUNT; i++)
        {
            //Calculate the XZ offsets for each of the 8 points in a circle: 
            float angleRadian = i * Mathf.PI * 2f / (float)RADIUS_GROUNDING_RAY_COUNT; //Get the radian value of each angle (0-2π) 
            Vector3 radiusRayOffset = new Vector3(Mathf.Cos(angleRadian) * characterController.radius, 0, Mathf.Sin(angleRadian) * characterController.radius); //Get point on the circumference at that angle

            Ray radiusRay = new Ray(transform.position + radiusRayOffset + allRaysVerticalOffset, Vector3.down);
            Debug.DrawLine(radiusRay.origin, radiusRay.origin + radiusRay.direction * groundedCheckDistance, Color.red);
        }
    }
}
