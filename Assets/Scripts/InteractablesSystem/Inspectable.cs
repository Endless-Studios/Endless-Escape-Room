using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inspectable : Interactable
{
    [SerializeField] Vector3 inspectionDefaultRotation = Vector3.zero;
    [SerializeField] float inspectDistance = 0;

    public UnityEvent OnInspectionStarted = new UnityEvent();
    public UnityEvent OnInspectionEnded = new UnityEvent();

    protected override string DefaultInteractionText => "Inspect";

    public Vector3 InspectionDefaultRotation => inspectionDefaultRotation;

    public float InspectDistance => inspectDistance;

    Quaternion initialRotation;
    Vector3 initialPosition;
    Transform initialParent;

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
