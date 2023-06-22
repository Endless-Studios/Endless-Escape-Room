using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
            {
                identifiers = new Identifier[pickupable.Identifiers.Length];
                System.Array.Copy(pickupable.Identifiers, identifiers, pickupable.Identifiers.Length);
            }
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
                return pickupable.GetHashCode();
            else
                return ((IStructuralEquatable)identifiers).GetHashCode(EqualityComparer<Identifier>.Default);
        }
    }

    public class StackableInventorySlot : InventorySlotBase
    {
        public List<Pickupable> items = new List<Pickupable>();
        string prompt = string.Empty;

        public override Pickupable Pickupable => items[0];
        public override int Count => items.Count;
        public override string Prompt => prompt;

        public void SetPrompt(string newPrompt)
        {
            prompt = newPrompt;
            HandleSlotUpdated();
        }
    }

    [SerializeField] CameraManager cameraManager;
    [SerializeField] PlayerInteractor interactor;
    [SerializeField] HeldItemManager heldItemManager;
    [SerializeField] ItemInspector itemInspector;
    [SerializeField] int maxInventorySlots = 99999;
    [SerializeField] int maxStackSize = 99;

    Dictionary<PickupableKey, StackableInventorySlot> heldItemsMap = new Dictionary<PickupableKey, StackableInventorySlot>();
    List<StackableInventorySlot> orderedSlots = new List<StackableInventorySlot>();

    int selectedIndex = -1;

    public override bool UiAlwaysOpen => true;

    private void Start()
    {
        //Maybe instead switch to a notification?
        interactor.OnItemInteracted.AddListener(HandleItemInteracted);
        PlayerHUD.Instance.InventoryUi.Show();
    }

    private void HandleItemInteracted(Interactable interactable)
    {
        if(interactable is Pickupable)
        {
            PickupItem(interactable as Pickupable);
        }
    }

    public override bool CanPickupItem(Pickupable pickupable)
    {
        PickupableKey key = new PickupableKey(pickupable);
        if(heldItemsMap.ContainsKey(key))
            return heldItemsMap[key].items.Count < maxStackSize;
        else
            return heldItemsMap.Count < maxInventorySlots;
    }

    public override InventorySlotBase[] GetItems()
    {
        return orderedSlots.ToArray();
    }

    public override bool PickupItem(Pickupable pickupable)
    {
        if(CanPickupItem(pickupable))
        {
            Debug.Log("Picking up item!");
            PickupableKey key = new PickupableKey(pickupable);
            if(heldItemsMap.ContainsKey(key) == false)
            {
                StackableInventorySlot newSlot = new StackableInventorySlot();
                orderedSlots.Add(newSlot);
                int newSlotIndex = orderedSlots.Count - 1;
                newSlot.SetPrompt(GetIndexPrompt(newSlotIndex));
                heldItemsMap[key] = newSlot;
                pickupable.OnDroppedInternal.AddListener(HandlePickupableDropped);
            }
            heldItemsMap[key].items.Add(pickupable);
            PlayerHUD.Instance.InventoryUi.Show();

            pickupable.gameObject.SetActive(false);
            //TODO subscribe to pickupable OnIdentifiersChanged to move to a new slot/update current slot
            pickupable.HandlePickedUp();
            return true;
        }
        return false;
    }

    private void HandlePickupableDropped(Pickupable droppedPickupable)
    {
        //Debug.Log("Dropped");
        droppedPickupable.OnDroppedInternal.RemoveListener(HandlePickupableDropped);
        PickupableKey key = new PickupableKey(droppedPickupable);
        StackableInventorySlot slot = heldItemsMap[key];
        slot.items.RemoveAt(0);

        if(slot.items.Count > 0)
        {
            //Debug.Log("Still carrying some");
            if(orderedSlots.IndexOf(slot) == selectedIndex)
            {//The thing dropped was in our "hand"
                if(cameraManager.IsFocused)
                {//If the camera is focused, we dont want to be spawning visuals, clear selection
                    ClearSelection();
                }
                else
                {//Show the next visual!
                    ShowCurrentSlot();
                }
            }

            //Subscribe to the next item in the slot for the next drop!
            slot.items[0].OnDroppedInternal.AddListener(HandlePickupableDropped);
            slot.HandleSlotUpdated();
        }
        else
        {
            //Debug.Log("out of stock");
            int indexToRemove = orderedSlots.IndexOf(slot);
            if(indexToRemove == selectedIndex)
            {
                selectedIndex = -1;
            }
            heldItemsMap.Remove(key);
            orderedSlots.RemoveAt(indexToRemove);
            UpdateOrderSlotPrompts();
            PlayerHUD.Instance.InventoryUi.Show();
        }
    }

    private void UpdateOrderSlotPrompts()
    {
        for (int index = 0; index < orderedSlots.Count; index++)
            orderedSlots[index].SetPrompt(GetIndexPrompt(index));
    }

    private static string GetIndexPrompt(int index)
    {
        if(index < 9)
            return (index + 1).ToString();
        else if(index == 9)
            return "0";
        else
            return string.Empty;
    }

    private void Update()
    {
        int startKey = (int)KeyCode.Alpha1;
        for(int keyValue = startKey; keyValue < startKey + 9 && keyValue - startKey < orderedSlots.Count; keyValue++)
        {
            if(Input.GetKeyDown((KeyCode)keyValue))
            {
                int index = keyValue - startKey;
                //Debug.Log($"Key {(KeyCode)keyValue} Pressed: {index}");
                if(selectedIndex != index)
                {
                    if(selectedIndex != -1)
                        ClearSelection();
                    selectedIndex = index;
                    ShowCurrentSlot();
                }
                else
                {
                    if(itemInspector.CurrentInspectable == null)
                    {
                        //If you're inspecting an item you cant deselect it, instead they should exit inspection if they need to
                        ClearSelection();
                    }
                }
            }
        }
    }

    private void ClearSelection()
    {
        if(heldItemManager.HeldPickupable)
            heldItemManager.HeldPickupable.gameObject.SetActive(false);
        heldItemManager.ClearHeldItem();
        selectedIndex = -1;
    }

    private void ShowCurrentSlot()
    {
        Pickupable pickupable = orderedSlots[selectedIndex].Pickupable;
        pickupable.gameObject.SetActive(true);
        heldItemManager.HoldItem(pickupable, itemInspector.IsInspecting);
        if(itemInspector.IsInspecting)
        {
            itemInspector.SwapCurrentInspectable(pickupable);
            PlayerHUD.Instance.InventoryUi.Show();
        }
    }
}
