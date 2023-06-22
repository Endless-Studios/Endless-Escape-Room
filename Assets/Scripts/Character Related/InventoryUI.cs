using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] UiInventoryElement itemUiPrefab = null;
    [SerializeField] UiInventoryElement draggedUiPrefab = null;
    [SerializeField] Canvas inventoryCanvas = null;
    [SerializeField] RectTransform inventoryEntriesParent = null;
    [SerializeField] float dropRaycastDistance = 5;
    [SerializeField] LayerMask dropRaycastMask;
    [SerializeField] bool addInReverseOrder = true;

    Snappable currentSnappable = null;
    UiInventoryElement originalElement = null;
    UiInventoryElement draggingElement = null;
    List<UiInventoryElement> currentEntries = new List<UiInventoryElement>();

    private void Awake()
    {
        inventoryCanvas.enabled = false;
    }

    public void Show()
    {
        if(inventoryCanvas.enabled)
        {
            //If we are already shown, lets clear and reinitiaize, the context could have changed!
            ClearEntries();
        }

        inventoryCanvas.enabled = true;

        InventorySlotBase[] inventorySlots = PlayerCore.LocalPlayer.Inventory.GetItems();
        if(addInReverseOrder)
        {
            for(int index = inventorySlots.Length - 1; index >= 0; index--)
            {
                SetupSlot(inventorySlots[index]);
            }
        }
        else
        {
            for(int index = 0; index < inventorySlots.Length; index++)
            {
                SetupSlot(inventorySlots[index]);
            }
        }
    }

    private void SetupSlot(InventorySlotBase slot)
    {
        UiInventoryElement newEntry = Instantiate(itemUiPrefab, inventoryEntriesParent);
        LayoutRebuilder.ForceRebuildLayoutImmediate(inventoryCanvas.transform as RectTransform);
        newEntry.Initialize(slot, PlayerCore.LocalPlayer.ItemInspector.CurrentInspectable != slot.Pickupable);
        currentEntries.Add(newEntry);
    }

    public void Hide()
    {
        if(PlayerCore.LocalPlayer.Inventory.UiAlwaysOpen == false)
        {
            if(inventoryCanvas.enabled)
            {
                inventoryCanvas.enabled = false;
                ClearEntries();
            }
        }
    }

    private void ClearEntries()
    {
        foreach(UiInventoryElement entry in currentEntries)
        {
            Destroy(entry.gameObject);
        }
        currentEntries = new List<UiInventoryElement>();
    }

    private void Update()
    {
        if(inventoryCanvas.enabled)
        {
            if(draggingElement == null)
            {
                if(Input.GetMouseButtonDown(0))
                {
                    GameObject hoveredObject = EventSystem.current.currentSelectedGameObject;
                    if(hoveredObject)
                    {
                        UiInventoryElement inventoryElement = hoveredObject.GetComponent<UiInventoryElement>();
                        if(inventoryElement)
                        {
                            originalElement = inventoryElement;
                            draggingElement = Instantiate(draggedUiPrefab, inventoryElement.transform.position, inventoryElement.transform.rotation, inventoryCanvas.transform);
                            draggingElement.Initialize(originalElement.Slot);
                            PlayerCore.LocalPlayer.PlayerInput.InspectControlsEnabled = false;
                        }
                    }
                }
            }
            else
            {
                Ray interactRay;
                if(MouseLockHandler.Instance.IsMouseLocked)
                    interactRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                else
                    interactRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                Snappable hitSnappable = null;
                RaycastHit hitInfo;
                bool didHit = Physics.Raycast(interactRay, out hitInfo, dropRaycastDistance, dropRaycastMask, QueryTriggerInteraction.Collide);
                if(didHit)
                {
                    hitSnappable = hitInfo.collider.GetComponentInParent<Snappable>();
                    if(hitSnappable && hitSnappable.AcceptsPickupable(originalElement.Slot.Pickupable) == false)
                    {//If we hit one, but it wont accept our object, ignore it.
                        hitSnappable = null;
                    }
                }
                if(Input.GetMouseButton(0))
                {//Button is held, this is a drag
                    draggingElement.transform.position = Input.mousePosition;

                    if(currentSnappable != hitSnappable)
                    {
                        if(hitSnappable == null)
                        {
                            draggingElement.Unhighlight();
                        }
                        else
                        {
                            draggingElement.Highlight();
                        }
                        currentSnappable = hitSnappable;
                    }
                }
                else
                {//We have released the button
                 //Did we do something with the pickupable because of our drop? (probably hit a snappable)
                    if(hitSnappable != null)
                    {
                        Pickupable droppedPickupable = originalElement.Slot.Pickupable;
                        if(PlayerCore.LocalPlayer.HeldItemManager.HeldPickupable == droppedPickupable)
                        {
                            PlayerCore.LocalPlayer.HeldItemManager.ClearHeldItem();
                        }

                        droppedPickupable.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0.5f));//TODO dont magic number?
                        droppedPickupable.gameObject.SetActive(true);
                        hitSnappable.SnapPickupable(droppedPickupable);
                        droppedPickupable.HandleDropped(false);

                        if(originalElement.Slot.Count <= 0)
                        {
                            currentEntries.Remove(originalElement);
                            Destroy(originalElement.gameObject);
                        }
                    }
                    //We're no longer dragging anything, lets clean up
                    Destroy(draggingElement.gameObject);
                    PlayerCore.LocalPlayer.PlayerInput.InspectControlsEnabled = true;

                    draggingElement = null;
                    originalElement = null;
                    currentSnappable = null;    
                }
            }
        }
    }
}
