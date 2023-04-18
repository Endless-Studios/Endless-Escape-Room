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

    internal void Unhighlight()
    {
        gameObject.layer = LayerMask.NameToLayer(normalLayer);
        isHighlighted = false;
    }

    internal void Highlight()
    {
        gameObject.layer = LayerMask.NameToLayer(highlightLayer);
        isHighlighted = true;
    }

    internal void Interact()
    {
        OnInteracted.Invoke();
    }
}
