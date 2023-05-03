using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private const int RADIUS_GROUNDING_RAY_COUNT = 8;

    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private PlayerInput playerInput = null;

    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 3f;
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private float terminalVelocity = -60;
    [SerializeField] private int jumpForce = 5; //Temp, probably replace with press and hold input?
    [SerializeField] private LayerMask groundedLayerMask;

    private float yVelocity = 0;
    private bool isGrounded;
    private Vector3 movementDampVelocity = Vector3.zero;
    private Vector3 motion;

    //TODO move into function, not as ternary operator
    float MoveSpeed => playerInput.GetSprintHeld() ? runSpeed : walkSpeed;

    // Start is called before the first frame update
    void Start()
    {
        float correctHeight = characterController.center.y + characterController.skinWidth;
        // set the controller center vector:
        characterController.center = new Vector3(0, correctHeight, 0);
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = CheckGrounding();

        if (isGrounded)
        {
            yVelocity = 0;
            if (playerInput.GetJumpRequested())
                yVelocity = jumpForce;//Temp simple jump
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
        motion = Vector3.SmoothDamp(characterController.velocity, moveInput * MoveSpeed, ref movementDampVelocity, accelerationTime);
        motion.y = yVelocity;
        characterController.Move(motion * Time.deltaTime);
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
