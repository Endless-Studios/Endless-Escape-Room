using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Vector3 moveDir;

    [SerializeField]
    private CharacterMovement characterMovement;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        moveDir.Normalize();


        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeMovement = moveDir.z * forward;
        Vector3 rightRelativeMovement = moveDir.x * right;

        Vector3 cameraRelativeMovement = forwardRelativeMovement + rightRelativeMovement;

        moveDir = cameraRelativeMovement;

        //moveDir.y = 0;

        characterMovement.Move(moveDir);

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
