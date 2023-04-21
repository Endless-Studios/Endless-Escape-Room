using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Inspectable
{
    [SerializeField] Rigidbody dropRigidbody;
    [SerializeField] Identifier[] identifiers;

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
    }

    internal void HandleDropped()
    {
        if(dropRigidbody)
            dropRigidbody.isKinematic = false;
        SetToNormalLayer();
    }
}
