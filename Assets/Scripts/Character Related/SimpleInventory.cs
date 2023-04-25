using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple implementation of inventory that holds a only single pickupable
/// </summary>
public class SimpleInventory : InventoryBase
{
    //TODO move all the held item stuff into a new class for holding an item
    //This "simple inventory" should just on pickup, store the single item, and tell the new class about the thing held, let it handle the rest. 
    //Keep this about storage, not holding/using/inspecting/dropping!

    [SerializeField] PlayerInput playerInput;
    [SerializeField] CharacterController controller;
    [SerializeField] Vector3 holdLocalPosition = Vector3.forward;
    [SerializeField] Vector3 holdLocalRotation = Vector3.zero;
    [SerializeField] float dropRaycastDistance = 5;
    [SerializeField] LayerMask dropRaycastMask;
    [SerializeField] ItemInspector itemInspector = null;
    [SerializeField] private float projectionSpeed = 1;

    Snappable currentSnappable = null;

    public override Pickupable HeldItem => heldPickupable;

    public override bool CanPickupItem(Pickupable pickupable)
    {
        return heldPickupable == null;
    }

    public override bool PickupItem(Pickupable pickupable)
    {
        if(CanPickupItem(pickupable))
        {
            heldPickupable = pickupable;
            heldUseable = pickupable as Useable;
            heldPickupable.HandlePickedUp();
            heldPickupable.transform.SetParent(Camera.main.transform, true);
            heldPickupable.transform.localPosition = holdLocalPosition;
            heldPickupable.transform.localRotation = Quaternion.Euler(holdLocalRotation);
            if(pickupable is Useable)
                PlayerHUD.Instance.SetHeldScreenActive(true, false);
            else
                ActivateDropMode();

            return true;
        }
        return false;
    }

    private void Update()
    {
        if(heldPickupable)
        {
            if(projectedVisuals)
                ProjectHeldItem();
            if(IsDropMode && playerInput.GetDropPressed())
            {
                //TODO only for visuals, not collisions until we're done maybe?
                heldPickupable.SetToNormalLayer();
                if(currentSnappable)
                {
                    currentSnappable.SnapPickupable(heldPickupable);
                    currentSnappable = null;
                }
                else
                {
                    heldPickupable.transform.parent = null;
                    heldPickupable.IsInteractable = true;
                    //TODO account for transform from visualRoot to parent
                    heldPickupable.transform.position = projectedVisuals.transform.position;
                    heldPickupable.transform.rotation = Quaternion.identity;
                    heldPickupable.HandleDropped();
                }

                ClearProjectedVisuals();
                PlayerHUD.Instance.SetHeldScreenActive(false);
                heldPickupable = null;
                heldUseable = null;
            }
            else if(itemInspector.IsInspecting == false && HeldItem != null && playerInput.GetInspectPressed())
            {
                PlayerHUD.Instance.SetHeldScreenActive(false);
                itemInspector.InspectItem(HeldItem);
            }
            else if (!IsDropMode && heldUseable && playerInput.GetInteractPressed())
            {
                heldUseable.Use();
            }
        }
    }

    private void ProjectHeldItem()
    {
        RaycastHit hitInfo;
        Vector3 targetPosition;
        Quaternion targetRotation;

        //TODO Move to a bounds projection maybe?
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, dropRaycastDistance, dropRaycastMask))
        {
            Snappable snappable = hitInfo.collider.GetComponent<Snappable>();
            if(snappable && snappable.AcceptsPickupable(heldPickupable))
            {
                currentSnappable = snappable;
                targetPosition = snappable.SnapTransform.position;
                targetRotation = snappable.SnapTransform.rotation;
            }
            else
            {
                currentSnappable = null;
                targetPosition = hitInfo.point;
                targetRotation = Quaternion.identity;
            }
        }
        else
        {
            currentSnappable = null;
            targetPosition = Camera.main.transform.position + Camera.main.transform.forward * dropRaycastDistance;
            targetRotation = Quaternion.identity;
        }
        projectedVisuals.transform.position = Vector3.Lerp(projectedVisuals.transform.position, targetPosition, Time.deltaTime * projectionSpeed);
        projectedVisuals.transform.rotation = Quaternion.Lerp(projectedVisuals.transform.rotation, targetRotation, Time.deltaTime * projectionSpeed);
    }
}
