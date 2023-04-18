using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController characterController = null;
    [SerializeField] private PlayerInput playerInput = null;

    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private float terminalVelocity = -60;
    [SerializeField] private int jumpForce = 5; //Temp, probably replace with press and hold input?

    private float yVelocity = 0;
    private bool isGrounded;
    private Vector3 movementDampVelocity = Vector3.zero;
    private Vector3 motion;

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
        Ray groundedRay = new Ray(transform.position + characterController.center + Vector3.up * (0.01f - characterController.height / 2f), Vector3.down);
        Debug.DrawLine(groundedRay.origin, groundedRay.origin + groundedRay.direction * 0.02f, Color.red);
        if(Physics.Raycast(groundedRay, 0.02f))
        {
            isGrounded = true;
            yVelocity = 0;
            if(playerInput.GetJumpRequested())
                yVelocity = jumpForce;//Temp simple jump
        }
        else
        { 
            isGrounded = false;
            yVelocity += Physics.gravity.y * Time.deltaTime;
            yVelocity = Mathf.Max(yVelocity, terminalVelocity);
        }

        Vector3 moveInput = playerInput.GetMovementInput();

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

        motion = Vector3.SmoothDamp(characterController.velocity, moveInput * walkSpeed, ref movementDampVelocity, accelerationTime);
        motion.y = yVelocity;

        characterController.Move(motion * Time.deltaTime);
    }
}
