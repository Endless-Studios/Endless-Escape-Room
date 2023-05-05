using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public void Show(InventoryBase inventoryBase)
    {
        if(inventoryCanvas.enabled)
        {
            //If we are already shown, lets clear and reinitiaize, the context could have changed!
            Hide();
        }

        Pickupable[] items = inventoryBase.GetHeldItems(null);
        foreach(Pickupable item in items)
        {
            UiInventoryElement newEntry = Instantiate(itemUiPrefab, inventoryEntriesParent);
            newEntry.Initialize(item);
            currentEntries.Add(newEntry);
        }
        inventoryCanvas.enabled = true;
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
            if(Input.GetKeyDown(KeyCode.F2)) //TODO TEMP
            {
                Hide();
            }
        }
        else if (Input.GetKeyDown(KeyCode.F1)) //TODO TEMP
        {
            Show(GameObject.FindObjectOfType<SimpleInventory>());
        }
    }
}
