using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] float jumpBufferTime = 0.25f;
    //TODO convert all axis to be SerializedFields

    public bool InteractEnabled { get; set; } = true;
    public bool LookEnabled => objectsBlockingLook.Count < 1;
    public bool MoveEnabled => objectsBlockingMovement.Count < 1;
    public bool HeldControlsEnabled { get; set; } = true;
    public bool InspectControlsEnabled { get; set; } = true;
    float lastJumpPressTime = -1;

    private List<object> objectsBlockingMovement = new List<object>();
    private List<object> objectsBlockingLook = new List<object>();

    private void Awake()
    {
        CinemachineCore.GetInputAxis = GetInputAxis;
    }

    public void BlockMovement(object blockingObject)
    {
        objectsBlockingMovement.Add(blockingObject);
    }

    public void UnblockMovement(object blockingObject)
    {
        objectsBlockingMovement.Remove(blockingObject);
    }

    public void BlockLook(object blockingObject)
    {
        objectsBlockingLook.Add(blockingObject);
    }

    public void UnblockLook(object blockingObject)
    {
        objectsBlockingLook.Remove(blockingObject);
    }

    private float GetInputAxis(string axisName)
    {
        if(LookEnabled)
            return Input.GetAxis(axisName);
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(MoveEnabled && Input.GetButtonDown("Jump"))
        {
            lastJumpPressTime = Time.time;
        }
    }

    internal Vector3 GetMovementInput()
    {
        if(MoveEnabled)
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
        return HeldControlsEnabled && Input.GetKeyDown(KeyCode.Tab);
    }

    public bool GetJumpRequested()
    {
        if(MoveEnabled)
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
        return InteractEnabled && (Input.GetButtonDown("Interact") || Input.GetButtonDown("Pickup"));
    }

    internal bool GetInteractReleased()
    {
        return Input.GetButtonUp("Interact") || Input.GetButtonUp("Pickup");
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
        return HeldControlsEnabled && Input.GetButtonDown("Drop");
    }

    public bool GetSprintHeld()
    {
        return Input.GetButton("Sprint");
    }

    internal bool GetRotationButtonUp()
    {
        return Input.GetButtonUp("Interact");
    }

    internal bool GetRotationButtonDown()
    {
        return InspectControlsEnabled && Input.GetButtonDown("Interact");
    }

    public bool GetUseButtonDown()
    {
        return HeldControlsEnabled && Input.GetButtonDown("Interact");
    }

    public bool GetCrouchHeld()
    {
        return MoveEnabled && Input.GetButton("Hold Crouch");
    }

    public bool GetToggleCrouchPressed()
    {
        return MoveEnabled && Input.GetButtonDown("Toggle Crouch");
    }
}
