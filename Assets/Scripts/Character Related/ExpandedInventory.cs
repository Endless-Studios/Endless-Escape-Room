using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExpandedInventory : InventoryBase
{
    /// <summary>
    /// Allows pickupables with matching identifiers to be stacked
    /// </summary>
    public class PickupableKey : System.IEquatable<PickupableKey>
    {
        Identifier[] identifiers = null;
        Pickupable pickupable = null;

        public PickupableKey(Pickupable pickupable)
        {
            if(pickupable.Identifiers.Length > 0)
                System.Array.Copy(pickupable.Identifiers, identifiers, pickupable.Identifiers.Length);
            else
                this.pickupable = pickupable;
        }

        public bool Equals(PickupableKey other)
        {
            if(other.identifiers != null)
            {
                if(identifiers != null)
                    return Enumerable.SequenceEqual(other.identifiers, identifiers);
                else
                    return false;
            }
            else if(other.pickupable != null && pickupable != null)
                return other.pickupable == pickupable;
            else
                return false;
        }

        public override int GetHashCode()
        {
            if(pickupable != null)
                return pickupable.GetHashCode()
            else
                return identifiers.GetHashCode();
        }
    }

    public class InventorySlot
    {
        public List<Pickupable> itemsHeld = new List<Pickupable>();
    }

    [SerializeField] int maxInventorySlots = 99999;
    [SerializeField] int maxStackSize = 99;

    //TODO unique key for a pickupable that is shared accross multiple to enable stacking?? Can we a list of identifiers as the unique key? Maybe hash the entire list?
    Dictionary<PickupableKey, InventorySlot> heldItems = new Dictionary<PickupableKey, InventorySlot>();

    public override bool CanPickupItem(Pickupable pickupable)
    {
        if(heldItems.ContainsKey(new PickupableKey(pickupable)))
        {

        }
        throw new System.NotImplementedException();
    }

    //Convert to InventorySlot? Maybe an interface that has a count, and pickupable?
    public override Pickupable[] GetItems(Pickupable[] skipList = null)
    {
        throw new System.NotImplementedException();
    }

    public override bool PickupItem(Pickupable pickupable)
    {
        if(CanPickupItem(pickupable))
        {
            //TODO subscribe to pickupable OnIdentifiersChanged to move to a new slot/update current slot
            return true;
        }
        return false;
    }
}
