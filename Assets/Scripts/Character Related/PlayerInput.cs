using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    //TODO convert all axis to be SerializedFields

    bool lookEnabled = true;
    bool moveEnabled = true;

    private void Awake()
    {
        CinemachineCore.GetInputAxis = GetInputAxis;
    }

    private float GetInputAxis(string axisName)
    {
        if(lookEnabled)
            return Input.GetAxis(axisName);
        return 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    internal Vector3 GetMovementInput()
    {
        if(moveEnabled)
        {
            Vector3 moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            moveDirection.Normalize();

            return moveDirection;
        }
        else
            return Vector3.zero;
    }

    internal bool GetInspectPressed()
    {
        //TODO switch to button
        return Input.GetKeyDown(KeyCode.Tab);
    }

    public bool GetJumpRequested()
    {
        return moveEnabled && Input.GetButton("Jump");
    }

    internal bool GetInteractPressed()
    {
        return Input.GetButtonDown("Fire1");
    }

    public void SetLookControlsActive(bool active)
    {
        lookEnabled = active;
    }

    public void SetMoveCotrolsActive(bool active)
    {
        moveEnabled = active;
    }

    public Vector2 GetMouseInput()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public bool GetBackPressed()
    {
        return Input.GetButtonDown("Cancel");
    }

    public bool GetPickupPressed()
    {
        return Input.GetButtonDown("Submit");
    }

    public bool GetDropPressed()
    {
        return Input.GetButtonDown("Fire2");
    }
}
