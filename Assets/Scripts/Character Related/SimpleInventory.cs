using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleInventorySlot : InventorySlotBase
{
    Pickupable item = null;

    public SimpleInventorySlot(Pickupable pickupable)
    {
        item = null;
    }

    public override Pickupable Pickupable => item;
    public override int Count => 1;
    public override string Prompt => string.Empty;
}

/// <summary>
/// A simple implementation of inventory that just checks if an item is held only
/// </summary>
public class SimpleInventory : InventoryBase
{
    [SerializeField] HeldItemManager heldItemManager;
    [SerializeField] ItemInspector itemInspector;
    [SerializeField] PlayerInteractor interactor;
    [SerializeField] bool inspectOnPickup = true;

    public override bool UiAlwaysOpen => false;

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

    public override InventorySlotBase[] GetItems()
    {//We're only every have 0 or 1 items in this simple inventory
        if(heldItemManager.HeldPickupable != null)
        {
            return new SimpleInventorySlot[] { new SimpleInventorySlot(heldItemManager.HeldPickupable) };
        }
        else
            return new SimpleInventorySlot[0];
    }
}
