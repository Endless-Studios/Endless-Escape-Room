using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Grabbable : Interactable
{
    [SerializeField] UnityEvent OnInteratStopped;

    protected override string DefaultInterationText => "Grab";

    internal void StopInteract()
    {
        HandleStopInteract();
        OnInteratStopped.Invoke();
    }

    protected abstract void HandleStopInteract();
}
