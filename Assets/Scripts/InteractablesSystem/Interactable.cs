using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum ItemLayer
{
    World,
    Held,
    Ui
}

[SelectionBase]
public class Interactable : MonoBehaviour
{
    [SerializeField] bool isInteractable = true;
    [SerializeField] string interactPromptText = string.Empty;
    [SerializeField] string itemName = string.Empty;
    public UnityEvent OnInteracted = new UnityEvent();

    protected virtual string DefaultInteractionText => "Interact";

    public bool IsInteractable 
    { 
        get => isInteractable; 
        set
        {
            //if(value == false && gameObject.layer == LayerMask.NameToLayer(highlightLayer))
                //Unhighlight(false);//Could cause issues with held items.
            isInteractable = value;
        }
    }

    public string InteractPrompt
    {
        get
        {
            if(string.IsNullOrWhiteSpace(interactPromptText))
            {
                if(string.IsNullOrWhiteSpace(itemName))
                    return DefaultInteractionText;
                else
                    return DefaultInteractionText + " " + itemName;
            }
            else
                return interactPromptText;
        }
        set
        {
            interactPromptText = value;
        }
    }

    const string worldHighlightLayer = "InteractableOutline";
    const string worldNormalLayer = "InteractableNoOutline";
    const string heldHighlightLayer = "InspectedOutline";
    const string heldNormalLayer = "InspectedItem";
    const string uiHighlightLayer = "UiOutline";
    const string uiNormalLayer = "UiNoOutline";

    static int GetHighlightedLayer(ItemLayer layer)
    {
        switch(layer)
        {
            case ItemLayer.World:
                return LayerMask.NameToLayer(worldHighlightLayer);
            case ItemLayer.Held:
                return LayerMask.NameToLayer(heldHighlightLayer);
            case ItemLayer.Ui:
                return LayerMask.NameToLayer(uiHighlightLayer);
            default:
                throw new System.ArgumentException($"You must have a standard {nameof(ItemLayer)} value!");
        }
    }

    static int GetNormalLayer(ItemLayer layer)
    {
        switch(layer)
        {
            case ItemLayer.World:
                return LayerMask.NameToLayer(worldNormalLayer);
            case ItemLayer.Held:
                return LayerMask.NameToLayer(heldNormalLayer);
            case ItemLayer.Ui:
                return LayerMask.NameToLayer(uiNormalLayer);
            default:
                throw new System.ArgumentException($"You must have a standard {nameof(ItemLayer)} value!");
        }
    }

    public void Unhighlight(ItemLayer layer = ItemLayer.World)
    {
        Unhighlight(transform, layer);
    }

    public static void Unhighlight(Transform transform, ItemLayer layer = ItemLayer.World)
    {
        //Debug.Log($"setting {gameObject.name} to a normal layer");
        SetLayerRecursive(transform, GetNormalLayer(layer));
    }

    public void Highlight(ItemLayer layer = ItemLayer.World)
    {
        Highlight(transform, layer);
    }

    public static void Highlight(Transform transform, ItemLayer layer = ItemLayer.World)
    {
        //Debug.Log($"setting {gameObject.name} to a highlight layer");
        SetLayerRecursive(transform, GetHighlightedLayer(layer));
    }

    static void SetLayerRecursive(Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        int childCount = transform.childCount;
        for(int childIndex = 0; childIndex < childCount; childIndex++)
            SetLayerRecursive(transform.GetChild(childIndex), layer);
    }

    internal void Interact()
    {
        OnInteracted.Invoke();
        InternalHandleInteract();
    }

    protected virtual void InternalHandleInteract()
    {
    }

    public void UpdatePrompt(string newPrompt)
    {
        interactPromptText = newPrompt;
    }
}
