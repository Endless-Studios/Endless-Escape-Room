using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple implementation of inventory that holds a only single pickupable
/// </summary>
public class SimpleInventory : InventoryBase
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] CharacterController controller;
    [SerializeField] Vector3 holdLocalPosition = Vector3.forward;
    [SerializeField] Vector3 holdLocalRotation = Vector3.zero;
    [SerializeField] float dropRaycastDistance = 5;
    [SerializeField] LayerMask dropRaycastMask;
    [SerializeField] Material dropIndicatorMaterial;

    Pickupable currentPickupable = null;
    GameObject projectedVisuals = null;

    public override Pickupable HeldItem => currentPickupable;

    public override bool CanPickupItem(Pickupable pickupable)
    {
        return currentPickupable == null;
    }

    public override bool PickupItem(Pickupable pickupable)
    {
        if(CanPickupItem(pickupable))
        {
            currentPickupable = pickupable;
            currentPickupable.HandlePickedUp();
            currentPickupable.transform.SetParent(Camera.main.transform, true);
            currentPickupable.transform.localPosition = holdLocalPosition;
            currentPickupable.transform.localRotation = Quaternion.Euler(holdLocalRotation);

            //TODO is there a better way to clone the object? We really only want renderers
            projectedVisuals = Instantiate(currentPickupable.VisualsRoot.gameObject);
            Renderer[] projectedRenderers = projectedVisuals.GetComponentsInChildren<Renderer>();
            foreach(Renderer renderer in projectedRenderers)
            {
                //TODO Modify layers to not draw on special camera (we want to see it in world space)
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.material = dropIndicatorMaterial;
            }
            return true;
        }
        return false;
    }

    private void Update()
    {
        if(currentPickupable)
        {
            ProjectHeldItem();
            if(playerInput.GetDropPressed())
            {
                currentPickupable.transform.parent = null;
                currentPickupable.IsInteractable = true;
                //TODO account for transform from visualRoot to parent
                currentPickupable.transform.position = projectedVisuals.transform.position;
                currentPickupable.transform.rotation = Quaternion.identity;
                currentPickupable.HandleDropped();
                Destroy(projectedVisuals);
                currentPickupable = null;
            }
        }
    }

    private void ProjectHeldItem()
    {
        RaycastHit hitInfo;
        //TODO Move to a bounds projection maybe?
        
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, dropRaycastDistance, dropRaycastMask))
            projectedVisuals.transform.position = hitInfo.point;
        else
            projectedVisuals.transform.position = Camera.main.transform.position + Camera.main.transform.forward * dropRaycastDistance;
    }
}
