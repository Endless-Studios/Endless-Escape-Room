using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inspectable : Interactable
{
    public UnityEvent OnInspectionStarted = new UnityEvent();
    public UnityEvent OnInspectionEnded = new UnityEvent();

    protected override string DefaultInteractionText => "Inspect";

    Quaternion initialRotation;
    Vector3 initialPosition;
    Transform initialParent;

    const string inspectedLayer = "InspectedItem";

    private void Awake()
    {
        CacheTransform();
    }

    public void HandleInspectionStarted()
    {
        OnInspectionStarted.Invoke();
    }

    public void HandleInspectionStopped()
    {
        OnInspectionEnded.Invoke();
    }

    protected void CacheTransform()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        initialParent = transform.parent;
    }

    public void RestoreTransform()
    {
        transform.SetParent(initialParent, true);
        transform.localPosition = initialPosition;
        transform.localRotation = initialRotation;
    }
}
