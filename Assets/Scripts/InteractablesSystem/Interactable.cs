using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    const string highlightLayer = "InteractableOutline";
    const string normalLayer = "InteractableNoOutline";
    const string inspectingHighlightLayer = "InspectedOutline";
    const string inspectingNormalLayer = "InspectedItem";

    public void Unhighlight(bool isInspecting)
    {
        SetToNormalLayer(isInspecting);
    }

    public void SetToNormalLayer(bool isInspecting = false)
    {
        //Debug.Log($"setting {gameObject.name} to a normal layer");
        SetLayerRecursive(transform, LayerMask.NameToLayer(isInspecting ? inspectingNormalLayer : normalLayer));
    }

    public void Highlight(bool isInspecting)
    {
        //Debug.Log($"setting {gameObject.name} to a highlight layer");
        SetLayerRecursive(transform, LayerMask.NameToLayer(isInspecting ? inspectingHighlightLayer : highlightLayer));
    }

    public static void SetLayerRecursive(Transform transform, int layer)
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
}
