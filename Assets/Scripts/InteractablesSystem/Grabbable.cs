using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Grabbable : Interactable
{
    public UnityEvent OnInteractStopped = new UnityEvent();

    protected override string DefaultInteractionText => "Grab";
    public abstract void HandleUpdate();

    internal void StopInteract()
    {
        HandleStopInteract();
        OnInteractStopped.Invoke();
    }

    protected abstract void HandleStopInteract();
}