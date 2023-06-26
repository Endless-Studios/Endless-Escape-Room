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

    [SerializeField] Vector3 holdOffset = Vector3.zero;
    [SerializeField] Vector3 holdRotation = new Vector3(0, 20, 0);

    protected override string DefaultInteractionText => "Pick up";

    public UnityEvent OnPickedUp = new UnityEvent();
    public UnityEvent OnDropped = new UnityEvent();
    [HideInInspector] public UnityEvent<Pickupable> OnDroppedInternal = new UnityEvent<Pickupable>();

    public Identifier[] Identifiers => identifiers;
    public GameObject VisualsPrefab => visualsPrefab;
    public Vector3 HoldOffset => holdOffset;
    public Vector3 HoldRotation => holdRotation;

    internal void HandlePickedUp()
    {
        //RestoreVisualsRoot();
        Unhighlight(ItemLayer.Held);
        if(dropRigidbody)
        {
            dropRigidbody.isKinematic = true;
        }
        OnPickedUp.Invoke();
    }

    internal void HandleDropped(bool enableRigidbody = true)
    {
        if(enableRigidbody && dropRigidbody)
        {
            dropRigidbody.isKinematic = false;
        }
        Unhighlight();
        OnDroppedInternal.Invoke(this);
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
