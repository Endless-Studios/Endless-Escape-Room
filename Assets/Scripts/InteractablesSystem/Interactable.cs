using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] bool isInteractable = true;
    [SerializeField] UnityEvent OnInteracted = new UnityEvent();

    public bool IsInteractable 
    { 
        get => isInteractable; 
        set
        {
            if(value == false && isHighlighted)
                Unhighlight();
            isInteractable = value;
        }
    }

    const string highlightLayer = "InteractableOutline";
    const string normalLayer = "InteractableNoOutline";

    bool isHighlighted = false;

    private void Awake()
    {
        Unhighlight();
    }

    internal void Unhighlight()
    {
        SetToNormalLayer();
        isHighlighted = false;
    }

    public void SetToNormalLayer()
    {
        SetLayerRecursive(transform, LayerMask.NameToLayer(normalLayer));
    }

    internal void Highlight()
    {
        SetLayerRecursive(transform, LayerMask.NameToLayer(highlightLayer));
        isHighlighted = true;
    }

    protected static void SetLayerRecursive(Transform transform, int layer)
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
