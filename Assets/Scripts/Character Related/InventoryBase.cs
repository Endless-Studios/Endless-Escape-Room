using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryBase : MonoBehaviour
{
    public abstract Pickupable HeldItem { get; }

    public abstract bool CanPickupItem(Pickupable pickupable);
    public abstract bool PickupItem(Pickupable pickupable);
}
