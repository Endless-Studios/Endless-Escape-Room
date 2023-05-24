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

    private float yVelocity = 0;
    private bool isGrounded;
    private Vector3 movementDampVelocity = Vector3.zero;
    private Vector3 motion;
    private bool crouchToggledOn = false;
    private float crouchStandMovementFactor; //calculated when character height is changed: 0 = full crouch | 1 = full stand

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
        isGrounded = CheckGrounding();

        if (isGrounded)
        {
            yVelocity = 0;
            if (playerInput.GetJumpRequested())
                yVelocity = Mathf.Lerp(crouchingJumpForce, jumpForce, crouchStandMovementFactor);//Temp simple jump
        }
        else
        {
            yVelocity += Physics.gravity.y * Time.deltaTime;
            yVelocity = Mathf.Max(yVelocity, terminalVelocity);
        }

        Vector3 moveInput = playerInput.GetMovementInput();
        if (moveInput != Vector3.zero)
        {
            //--Could go either way on this. Does playerInput communicate direction it intends to move? Or just wrapping input and letting character movement determine how to utilize that input?
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
            //--
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


        motion = Vector3.SmoothDamp(characterController.velocity, moveInput * CalculatedMovementSpeed, ref movementDampVelocity, accelerationTime);
        motion.y = yVelocity;
        characterController.Move(motion * Time.deltaTime);
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
        float centerHeight = targetHeight / 2f;
        characterController.height = targetHeight;
        characterController.center = new Vector3(0, centerHeight, 0);
        PlayerCore.LocalPlayer.FpsCameraRootTransform.localPosition = new Vector3(0, targetHeight + fpsCameraHeight, 0);
        characterController.stepOffset = Mathf.Lerp(crouchingStepOffset, standingStepOffset, crouchStandMovementFactor);
    }


    private bool CheckGrounding()
    {
        Vector3 allRaysVerticalOffset = characterController.center + Vector3.up * (0.01f - characterController.height / 2f);

        //Center grounding ray check
        Ray centerGroundedRay = new Ray(transform.position + allRaysVerticalOffset, Vector3.down);
        Debug.DrawLine(centerGroundedRay.origin, centerGroundedRay.origin + centerGroundedRay.direction * 0.02f, Color.red);

        if (Physics.Raycast(centerGroundedRay, 0.02f, groundedLayerMask))
        {
            return true;
        }

        //Radius grounding rays check
        for (int i = 0; i < RADIUS_GROUNDING_RAY_COUNT; i++)
        {
            //Calculate the XZ offsets for each of the 8 points in a circle: 
            float angleRadian = i * Mathf.PI * 2f / (float)RADIUS_GROUNDING_RAY_COUNT; //Get the radian value of each angle (0-2π) 
            Vector3 radiusRayOffset = new Vector3(Mathf.Cos(angleRadian) * characterController.radius, 0, Mathf.Sin(angleRadian) * characterController.radius); //Get point on the circumference at that angle

            Ray radiusRay = new Ray(transform.position + radiusRayOffset + allRaysVerticalOffset, Vector3.down);
            Debug.DrawLine(radiusRay.origin, radiusRay.origin + radiusRay.direction * 0.02f, Color.red);

            if (Physics.Raycast(radiusRay, 0.02f, groundedLayerMask))
            {
                return true;
            }
        }

        return false;
    }
}
