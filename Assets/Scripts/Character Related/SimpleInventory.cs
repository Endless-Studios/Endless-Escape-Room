using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A simple implementation of inventory that just checks if an item is held only
/// </summary>
public class SimpleInventory : InventoryBase
{
    [SerializeField] HeldItemManager heldItemManager;
    [SerializeField] ItemInspector itemInspector;
    [SerializeField] PlayerInteractor interactor;
    [SerializeField] bool inspectOnPickup = true;

    private void Start()
    {
        //Maybe instead switch to a notification?
        interactor.OnItemInteracted.AddListener(HandleItemInteracted);
    }

    private void HandleItemInteracted(Interactable interactable)
    {
        if(interactable is Inspectable)
        {
            PickupItem(interactable as Pickupable);
        }
    }

    public override bool CanPickupItem(Pickupable pickupable)
    {
        return heldItemManager.HeldPickupable == null && pickupable != null;
    }

    public override bool PickupItem(Pickupable pickupable)
    {
        if(CanPickupItem(pickupable))
        {
            pickupable.HandlePickedUp();
            heldItemManager.HoldItem(pickupable, inspectOnPickup);
            if(inspectOnPickup)
                itemInspector.InspectItem(pickupable);
            return true;
        }
        return false;
    }

    public override Pickupable[] GetHeldItems(Pickupable[] skipList = null)
    {//We're only every have 0 or 1 items in this simple inventory. Just check it the skip list contains the held item or not
        if(heldItemManager.HeldPickupable != null)
        {
            if(skipList != null && skipList.Contains(heldItemManager.HeldPickupable))
                return new Pickupable[0];
            else
                return new Pickupable[] { heldItemManager.HeldPickupable };
        }
        else
            return new Pickupable[0];
    }
}
