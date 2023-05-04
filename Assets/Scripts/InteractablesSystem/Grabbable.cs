using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Grabbable : Interactable
{
    [SerializeField] UnityEvent OnInteratStopped;

    protected override string DefaultInteractionText => "Grab";
    public abstract void HandleUpdate();

    internal void StopInteract()
    {
        HandleStopInteract();
        OnInteratStopped.Invoke();
    }

    protected abstract void HandleStopInteract();
}