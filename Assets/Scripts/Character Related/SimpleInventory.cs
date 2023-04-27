using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple implementation of inventory that just checks if an item is held only
/// </summary>
public class SimpleInventory : InventoryBase
{
    [SerializeField] HeldItemManager heldItemManager;

    public override bool CanPickupItem(Pickupable pickupable)
    {
        return heldItemManager.HeldPickupable == null;
    }

    public override bool PickupItem(Pickupable pickupable)
    {
        if(CanPickupItem(pickupable))
        {
            pickupable.HandlePickedUp();
            heldItemManager.HoldItem(pickupable);
            return true;
        }
        return false;
    }
}
