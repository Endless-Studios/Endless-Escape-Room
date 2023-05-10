using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] UiInventoryElement itemUiPrefab = null;
    [SerializeField] Canvas inventoryCanvas = null;
    [SerializeField] RectTransform inventoryEntriesParent = null;
    [SerializeField] float dropRaycastDistance = 5;
    [SerializeField] LayerMask dropRaycastMask;

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
            Hide();
        }

        inventoryCanvas.enabled = true;

        Pickupable[] items = PlayerCore.LocalPlayer.Inventory.GetItems(null);//TODO ignore the one in your hand actively, if inspecting
        foreach(Pickupable item in items)
        {
            UiInventoryElement newEntry = Instantiate(itemUiPrefab, inventoryEntriesParent);
            LayoutRebuilder.ForceRebuildLayoutImmediate(inventoryCanvas.transform as RectTransform);
            newEntry.Initialize(item);
            currentEntries.Add(newEntry);
        }
    }

    public void Hide()
    {
        if(inventoryCanvas.enabled)
        {
            inventoryCanvas.enabled = false;
            foreach(UiInventoryElement entry in currentEntries)
            {
                Destroy(entry.gameObject);
            }
            currentEntries = new List<UiInventoryElement>();
        }
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
                            draggingElement = Instantiate(itemUiPrefab, inventoryElement.transform.position, inventoryElement.transform.rotation, inventoryCanvas.transform);
                            draggingElement.GetComponent<Image>().enabled = false; //TODO remove this. Maybe have the initial prefabs in an off state, and then have initialize take an argument for dragging and let them handle it?
                            draggingElement.Initialize(originalElement.Pickupable);
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
                    if(hitSnappable && hitSnappable.AcceptsPickupable(originalElement.Pickupable) == false)
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
                        if(PlayerCore.LocalPlayer.HeldItemManager.HeldPickupable == originalElement.Pickupable)
                        {
                            PlayerCore.LocalPlayer.HeldItemManager.ClearHeldItem();
                        }

                        originalElement.Pickupable.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0.5f));//TODO dont magic number?
                        originalElement.Pickupable.gameObject.SetActive(true);
                        hitSnappable.SnapPickupable(originalElement.Pickupable);
                        originalElement.Pickupable.HandleDropped(false);

                        currentEntries.Remove(originalElement);
                        Destroy(originalElement.gameObject);
                    }

                    //We're no longer dragging anything, lets clean up
                    Destroy(draggingElement.gameObject);
                    draggingElement = null;
                    originalElement = null;
                    currentSnappable = null;    
                }
            }
        }
    }
}
