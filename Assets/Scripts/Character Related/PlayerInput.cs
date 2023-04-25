using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] float jumpBufferTime = 0.25f;
    //TODO convert all axis to be SerializedFields

    bool lookEnabled = true;
    bool moveEnabled = true;
    float lastJumpPressTime = -1;

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
        //TODO move elsewhere
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        if(moveEnabled && Input.GetButtonDown("Jump"))
        {
            lastJumpPressTime = Time.time;
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
        if(moveEnabled)
        {
            bool timingValid = lastJumpPressTime + jumpBufferTime >= Time.time;
            lastJumpPressTime = -1;
            return timingValid;
        }
        else
            return false;
    }

    internal bool GetInteractPressed()
    {
        return Input.GetButtonDown("Interact") || Input.GetButtonDown("Pickup");
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
        return Input.GetButtonDown("Cancel") || Input.GetButton("Drop");
    }

    public bool GetPickupPressed()
    {
        return Input.GetButtonDown("Pickup");
    }

    public bool GetDropPressed()
    {
        return Input.GetButtonDown("Drop");
    }

    public bool GetSprintHeld()
    {
        return Input.GetButton("Sprint");
    }
}
