using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum RequirementType
{
    Any,
    All
}

public class Snappable : MonoBehaviour
{
    [SerializeField] Identifier[] identifiers;
    [SerializeField] RequirementType requirementType = RequirementType.Any;
    [SerializeField] private float snapMoveTime = 0.25f; //TODO this should be generic across all snappables probably?

    [SerializeField] Transform attachTransform;
    [SerializeField] bool isObjectUnsnappble = true;

    public UnityEvent<Pickupable> OnObjectSnapped = new UnityEvent<Pickupable>();
    public UnityEvent OnObjectRemoved = new UnityEvent();

    public Transform SnapTransform => attachTransform ?? transform;

    Pickupable currentPickupable = null;
    public bool HasSnappedObject => currentPickupable;

    internal bool AcceptsPickupable(Pickupable pickupable)
    {
        if(currentPickupable)
            return false;
        switch(requirementType)
        {
            case RequirementType.Any:
                if(identifiers.Length == 0)
                    return true;
                foreach(Identifier identifier in identifiers)
                {
                    if(pickupable.Identifiers.Contains(identifier))
                        return true;
                }
                return false;
            case RequirementType.All:
                foreach(Identifier identifier in identifiers)
                {
                    if(pickupable.Identifiers.Contains(identifier) == false)
                        return false;
                }
                return true;
            default:
                return false;
        }
    }

    internal void SnapPickupable(Pickupable pickupable)
    {
        currentPickupable = pickupable;
        StartCoroutine(SnapPickupable());
    }

    private void HandleCurrentPickupableRemoved()
    {
        currentPickupable = null;
        OnObjectRemoved.Invoke();
    }

    IEnumerator SnapPickupable()
    {
        Vector3 startPosition = currentPickupable.transform.position;
        Quaternion startRotation = currentPickupable.transform.rotation;
        currentPickupable.transform.SetParent(SnapTransform, true);
        for(float elapsedTime = 0; elapsedTime < snapMoveTime; elapsedTime += Time.deltaTime)
        {
            currentPickupable.transform.position = Vector3.Slerp(startPosition, SnapTransform.position, elapsedTime / snapMoveTime);
            currentPickupable.transform.localRotation = Quaternion.Slerp(startRotation, SnapTransform.rotation, elapsedTime / snapMoveTime);
            yield return null;
        }
        currentPickupable.transform.position = SnapTransform.position;
        currentPickupable.transform.rotation = SnapTransform.rotation;
        yield return null;
        OnObjectSnapped.Invoke(currentPickupable);
        if(isObjectUnsnappble)
        {
            MakeObjectUnsnappable();
        }
    }

    public void MakeObjectUnsnappable()
    {
        currentPickupable.IsInteractable = true;
        currentPickupable.OnPickedUp.AddListener(HandleCurrentPickupableRemoved);
    }
}
