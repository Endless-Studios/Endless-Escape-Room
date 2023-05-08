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
    
    UiInventoryElement originalElement = null;
    RectTransform draggingElement = null;
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
                            UiInventoryElement newDragElement = Instantiate(inventoryElement, inventoryElement.transform.position, inventoryElement.transform.rotation, inventoryCanvas.transform);
                            newDragElement.Initialize(originalElement.Pickupable);
                            draggingElement = newDragElement.GetComponent<RectTransform>();
                        }
                    }
                }
            }
            else
            {
                if(Input.GetMouseButton(0))
                {//Button is held, this is a drag
                    draggingElement.position = Input.mousePosition;
                }
                else
                {//We have released the button
                    //Did we do something with the pickupable because of our drop? (probably hit a snappable)
                    if(false)
                    {
                        currentEntries.Remove(originalElement);
                        Destroy(originalElement.gameObject);
                    }

                    //We're no longer dragging anything, lets clean up
                    Destroy(draggingElement.gameObject);
                    draggingElement = null;
                    originalElement = null;
                }
            }
        }
    }
}
