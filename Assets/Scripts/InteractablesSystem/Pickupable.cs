using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickupable : Inspectable
{
    [SerializeField] Rigidbody dropRigidbody;
    [SerializeField] Identifier[] identifiers;
    [SerializeField] GameObject visualsPrefab;

    protected override string DefaultInterationText => "Pick up";

    public UnityEvent OnPickedUp = new UnityEvent();
    [SerializeField] UnityEvent OnDropped = new UnityEvent();

    public Identifier[] Identifiers => identifiers;
    public GameObject VisualsPrefab => visualsPrefab;

    internal void HandlePickedUp()
    {
        //RestoreVisualsRoot();
        SetToNormalLayer(true);
        if(dropRigidbody)
        {
            dropRigidbody.isKinematic = true;
            dropRigidbody.velocity = Vector3.zero;
            dropRigidbody.angularVelocity = Vector3.zero;
        }
        OnPickedUp.Invoke();
    }

    internal void HandleDropped(bool enableRigidbody = true)
    {
        if(enableRigidbody && dropRigidbody)
            dropRigidbody.isKinematic = false;
        SetToNormalLayer();
        OnDropped.Invoke();
    }
}
