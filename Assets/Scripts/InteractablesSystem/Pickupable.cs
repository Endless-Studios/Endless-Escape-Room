using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pickupable : Inspectable
{
    [Tooltip("Optional, does this object use physics?")]
    [SerializeField] Rigidbody dropRigidbody;
    [Tooltip("What type of object is this?")]
    [SerializeField] Identifier[] identifiers;
    [Tooltip("Optional, but more efficient when projecting the dropped visuals if it is prebuilt")]
    [SerializeField] GameObject visualsPrefab;

    protected override string DefaultInteractionText => "Pick up";

    public UnityEvent OnPickedUp = new UnityEvent();
    public UnityEvent OnDropped = new UnityEvent();

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

    protected virtual void OnValidate()
    {
        if(dropRigidbody == null)
            dropRigidbody = GetComponent<Rigidbody>();
    }
}
