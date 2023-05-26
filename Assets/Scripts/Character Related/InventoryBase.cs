using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class InventorySlotBase
{
    public UnityEvent OnSlotUpdated = new UnityEvent();
    public abstract Pickupable Pickupable { get; }
    public abstract int Count { get; }
    public abstract string Prompt { get; }

    public void HandleSlotUpdated()
    {
        OnSlotUpdated.Invoke();
    }
}

public abstract class InventoryBase : MonoBehaviour
{
    public abstract bool UiAlwaysOpen { get; }

    public abstract bool CanPickupItem(Pickupable pickupable);
    public abstract bool PickupItem(Pickupable pickupable);

    public abstract InventorySlotBase[] GetItems();
}
