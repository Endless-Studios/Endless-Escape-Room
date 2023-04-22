using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInspector : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor = null;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InventoryBase inventory;
    [SerializeField] private float attachOffset = 2;
    [SerializeField] private float inspectMoveTime = 0.5f;
    [SerializeField] private float rotationSpeed = 30;

    Inspectable currentInspectable = null;
    Coroutine inspectCoroutine = null;

    public bool IsInspecting => currentInspectable != false;

    private void Start()
    {
        //Maybe instead switch to a notification?
        interactor.OnItemInteracted.AddListener(HandleItemInteracted);
    }

    private void HandleItemInteracted(Interactable interactable)
    {
        if(interactable is Inspectable)
        {
            InspectItem(interactable as Inspectable);
        }
    }

    internal void InspectItem(Inspectable inspectable)
    {
        if(currentInspectable != null)
        {
            Debug.LogError("Cannot interact with another inspectable while inspecting!");
            return;
        }
        currentInspectable = inspectable;
        inspectCoroutine = StartCoroutine(InspectInspectable());
    }

    IEnumerator InspectInspectable()
    {
        Pickupable currentPickupable = currentInspectable as Pickupable;
        bool inspectingHeldItem = currentPickupable && inventory.HeldItem == currentPickupable;

        currentInspectable.IsInteractable = false;
        playerInput.SetLookControlsActive(false);
        playerInput.SetMoveCotrolsActive(false);
        currentInspectable.SetToHeldLayer();
        Vector3 startPosition = currentInspectable.VisualsRoot.transform.position;
        Quaternion startRotation = currentInspectable.VisualsRoot.transform.localRotation;
        currentInspectable.VisualsRoot.transform.SetParent(Camera.main.transform, true);
        for(float elapsedTime = 0; elapsedTime < inspectMoveTime; elapsedTime += Time.deltaTime)
        {
            currentInspectable.VisualsRoot.transform.position = Vector3.Slerp(startPosition, Camera.main.transform.position + Camera.main.transform.forward * attachOffset, elapsedTime / inspectMoveTime);
            currentInspectable.VisualsRoot.transform.localRotation = Quaternion.Slerp(startRotation, Quaternion.identity, elapsedTime / inspectMoveTime);
            yield return null;
        }
        currentInspectable.VisualsRoot.transform.position = Camera.main.transform.position + Camera.main.transform.forward * attachOffset;
        currentInspectable.VisualsRoot.transform.localRotation = Quaternion.identity;
        currentInspectable.VisualsRoot.transform.SetParent(null, true);
        bool backPressed = false;
        bool pickupPressed = false;

        while(backPressed == false && pickupPressed == false)
        {
            Vector2 mouseInput = playerInput.GetMouseInput();
            currentInspectable.VisualsRoot.transform.Rotate(Camera.main.transform.up, -mouseInput.x * Time.deltaTime * rotationSpeed);
            currentInspectable.VisualsRoot.transform.Rotate(Camera.main.transform.right, mouseInput.y * Time.deltaTime * rotationSpeed);
            yield return null;
            backPressed = playerInput.GetBackPressed();
            pickupPressed = currentPickupable != null && playerInput.GetPickupPressed() && (CanPickupItem(currentPickupable) || inspectingHeldItem);
        }
        if(inspectingHeldItem)
        {
            currentInspectable.RestoreVisualsRoot();
            if(backPressed)
                inventory.ActivateDropMode();
            else if(pickupPressed)
            {
                //enter use mode if usable, otherwise, drop mode
                if(currentPickupable is Useable == false)
                    inventory.ActivateDropMode();
            }
        }
        else
        {
            if(backPressed)
            {//TODO maybe move some of this into inspectable?
                currentInspectable.RestoreVisualsRoot();
                currentInspectable.SetToNormalLayer();
                currentInspectable.IsInteractable = true;
            }
            else if(pickupPressed)
            {
                inventory.PickupItem(currentPickupable);
            }
        }
        yield return null;
        currentInspectable = null;
        playerInput.SetLookControlsActive(true);
        playerInput.SetMoveCotrolsActive(true);
        inspectCoroutine = null;
    }

    bool CanPickupItem(Pickupable pickupable)
    {
        return inventory != null && inventory.CanPickupItem(pickupable);
    }
}
