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

    Pickupable currentPickupable = null;

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
            currentPickupable.transform.SetParent(Camera.main.transform, true);
            currentPickupable.transform.localPosition = holdLocalPosition;
            currentPickupable.transform.localRotation = Quaternion.Euler(holdLocalRotation);
            //TODO pickupable should know all the relevant colliders or do another layer swap?
            Physics.IgnoreCollision(controller, currentPickupable.GetComponent<Collider>(), true);
            return true;
        }
        return false;
    }

    private void Update()
    {
        if(currentPickupable)
        {
            if(playerInput.GetDropPressed())
            {
                currentPickupable.transform.parent = null;
                currentPickupable.IsInteractable = true;
                currentPickupable.HandleDropped();
                //TODO pickupable should know all the relevant colliders or do another layer swap?
                Physics.IgnoreCollision(controller, currentPickupable.GetComponent<Collider>(), false);
                currentPickupable = null;
            }
        }
    }
}
