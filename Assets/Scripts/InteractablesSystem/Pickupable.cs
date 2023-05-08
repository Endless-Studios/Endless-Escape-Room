using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    protected override string DefaultInterationText => "Pick up";

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

    internal GameObject GetVisualClone(Vector3 position, Quaternion rotation)
    {
        if(VisualsPrefab != null)
            return Instantiate(VisualsPrefab, position, rotation);
        else
        {//If they didnt have one, we can duplciate it and strip components. Less efficient, but more learner friendly
            GameObject manufacturedPrefab = Instantiate(gameObject, position, rotation);
            StripInvalidComponents(manufacturedPrefab.transform);
            return manufacturedPrefab;
        }
    }

    internal GameObject GetVisualCloneNoRotation()
    {
        if(VisualsPrefab != null)
            return Instantiate(VisualsPrefab, transform.position, Quaternion.identity);
        else
        {//If they didnt have one, we can duplciate it and strip components. Less efficient, but more learner friendly
            GameObject manufacturedPrefab = Instantiate(gameObject, transform.position, Quaternion.identity);
            StripInvalidComponents(manufacturedPrefab.transform);
            return manufacturedPrefab;
        }
    }

    static void StripInvalidComponents(Transform currentTransform)
    {
        Component[] components = currentTransform.GetComponents<Component>();
        System.Type[] validTypes = new System.Type[] { typeof(Transform), typeof(SkinnedMeshRenderer), typeof(MeshRenderer), typeof(MeshFilter) };
        foreach(Component component in components)
        {
            System.Type type = component.GetType();
            if(validTypes.Contains(type) == false)
                Destroy(component);
        }

        int childCount = currentTransform.childCount;
        for(int childIndex = 0; childIndex < childCount; childIndex++)
            StripInvalidComponents(currentTransform.GetChild(childIndex));
    }
}
