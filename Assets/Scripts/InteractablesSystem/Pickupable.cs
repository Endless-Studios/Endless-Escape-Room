using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickupable : Inspectable
{
    [SerializeField] Rigidbody dropRigidbody;
    [SerializeField] Identifier[] identifiers;

    protected override string DefaultInteractionText => "Pick up";

    public UnityEvent OnPickedUp = new UnityEvent();

    public Identifier[] Identifiers { get => identifiers; }

    internal void HandlePickedUp()
    {
        RestoreVisualsRoot();
        SetToHeldLayer();
        if(dropRigidbody)
        {
            dropRigidbody.isKinematic = true;
            dropRigidbody.velocity = Vector3.zero;
            dropRigidbody.angularVelocity = Vector3.zero;
        }
        OnPickedUp.Invoke();
    }

    internal void HandleDropped()
    {
        if(dropRigidbody)
            dropRigidbody.isKinematic = false;
        SetToNormalLayer();
    }
}
