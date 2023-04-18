using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public CharacterController characterController;

    [SerializeField]
    private float walkSpeed = 1f;
    [SerializeField]
    private float acceleration = 1f;

    private bool isGrounded;

    private Vector3 velocity = Vector3.zero;
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
        if (Physics.Raycast(transform.position + Vector3.up * 0.01f, Vector3.down, 0.02f))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void Move(Vector3 moveDir)
    {
        characterController.Move(Vector3.SmoothDamp(characterController.velocity, moveDir * walkSpeed, ref velocity, acceleration) * Time.deltaTime);
    }
}
