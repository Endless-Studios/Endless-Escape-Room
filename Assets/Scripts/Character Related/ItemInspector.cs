using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInspector : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor = null;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InventoryBase inventory;
    [SerializeField] private HeldItemManager heldItemManager;
    [SerializeField] private float attachOffset = 2;
    [SerializeField] private float inspectMoveTime = 0.5f;
    [SerializeField] private float rotationSpeed = 30;

    Coroutine inspectCoroutine = null;

    public Inspectable CurrentInspectable { get; private set; }
    public bool IsInspecting => CurrentInspectable != false;

    private void Start()
    {
        //Maybe instead switch to a notification?
        interactor.OnItemInteracted.AddListener(HandleItemInteracted);
    }

    private void HandleItemInteracted(Interactable interactable)
    {
        if(interactable is Inspectable && interactable is not Pickupable)
        {
            InspectItem(interactable as Inspectable);
        }
    }

    internal void InspectItem(Inspectable inspectable)
    {
        if(CurrentInspectable != null)
        {
            Debug.LogError("Cannot interact with another inspectable while inspecting!");
            return;
        }
        CurrentInspectable = inspectable;
        inspectCoroutine = StartCoroutine(InspectInspectable());
    }

    IEnumerator InspectInspectable()
    {
        CurrentInspectable.HandleInspectionStarted();
        Pickupable currentPickupable = CurrentInspectable as Pickupable;
        bool inspectingHeldItem = currentPickupable && heldItemManager.HeldPickupable == currentPickupable;

        CurrentInspectable.IsInteractable = false;
        playerInput.InteractEnabled = true;
        playerInput.LookEnabled = false;
        playerInput.MoveEnabled = false;
        playerInput.HeldControlsEnabled = false;
        CurrentInspectable.SetToNormalLayer(true);
        Vector3 startPosition = CurrentInspectable.transform.position;
        Quaternion startRotation = CurrentInspectable.transform.localRotation;
        for(float elapsedTime = 0; elapsedTime < inspectMoveTime; elapsedTime += Time.deltaTime)
        {
            CurrentInspectable.transform.position = Vector3.Slerp(startPosition, Camera.main.transform.position + Camera.main.transform.forward * attachOffset, elapsedTime / inspectMoveTime);
            CurrentInspectable.transform.localRotation = Quaternion.Slerp(startRotation, Quaternion.identity, elapsedTime / inspectMoveTime);
            yield return null;
        }
        CurrentInspectable.transform.position = Camera.main.transform.position + Camera.main.transform.forward * attachOffset;
        CurrentInspectable.transform.localRotation = Quaternion.identity;
        bool backPressed = false;
        bool pickupPressed = false;
        MouseLockHandler.Instance.ClaimMouseCursor(this);
        PlayerHUD.Instance.SetHeldScreenActive(false);
        bool canPickup = currentPickupable != null && (inventory.CanPickupItem(currentPickupable) || heldItemManager.HeldPickupable == currentPickupable);
        PlayerHUD.Instance.SetInspectScreenActive(true, canPickup);
        bool rotationHeld = false;
        while(backPressed == false && pickupPressed == false)
        {
            if(rotationHeld == false && playerInput.GetRotationButtonDown() && interactor.HasTarget == false)
            {
                rotationHeld = true;
                playerInput.InteractEnabled = false;
                //MouseLockHandler.Instance.ReleaseMouseCursor(this);
            }
            else if (rotationHeld && playerInput.GetRotationButtonUp())
            {
                rotationHeld = false;
                playerInput.InteractEnabled = true;
                //MouseLockHandler.Instance.ClaimMouseCursor(this);
            }
            if(rotationHeld)
            {
                Vector2 mouseInput = playerInput.GetMouseInput();
                CurrentInspectable.transform.RotateAround(CurrentInspectable.transform.position, Camera.main.transform.up, -mouseInput.x * Time.deltaTime * rotationSpeed);
                CurrentInspectable.transform.RotateAround(CurrentInspectable.transform.position, Camera.main.transform.right, mouseInput.y * Time.deltaTime * rotationSpeed);
            }
            yield return null;
            backPressed = playerInput.GetBackPressed();
            pickupPressed = currentPickupable != null && playerInput.GetPickupPressed() && (CanPickupItem(currentPickupable) || inspectingHeldItem);
        }
        if(inspectingHeldItem)
        {
            heldItemManager.MoveHeldItemToProperPosition();
            //CurrentInspectable.RestoreVisualsRoot();
            if(backPressed)
                heldItemManager.ActivateDropMode();
            else if(pickupPressed)
            {
                //enter use mode if usable, otherwise, drop mode
                if(currentPickupable is Useable == false)
                    heldItemManager.ActivateDropMode();
                else
                    heldItemManager.ReenterHeld();
            }
        }
        else
        {
            if(heldItemManager.HeldPickupable)
                heldItemManager.ReenterHeld();
            playerInput.InteractEnabled = true;
            if(backPressed)
            {//TODO maybe move some of this into inspectable?
                CurrentInspectable.RestoreTransform();
                CurrentInspectable.SetToNormalLayer();
                CurrentInspectable.IsInteractable = true;
            }
            else if(pickupPressed)
            {
                inventory.PickupItem(currentPickupable);
            }
        }
        PlayerHUD.Instance.SetInspectScreenActive(false);
        MouseLockHandler.Instance.ReleaseMouseCursor(this);
        CurrentInspectable.HandleInspectionStopped();

        yield return null;
        CurrentInspectable = null;
        playerInput.LookEnabled = true;
        playerInput.MoveEnabled = true;
        inspectCoroutine = null;
    }

    bool CanPickupItem(Pickupable pickupable)
    {
        return inventory != null && inventory.CanPickupItem(pickupable);
    }
}
