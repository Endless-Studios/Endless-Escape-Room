using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inspectable : Interactable
{
    [SerializeField] UnityEvent OnInspectionStarted = new UnityEvent();
    [SerializeField] UnityEvent OnInspectionEnded = new UnityEvent();

    [SerializeField] Transform visualsRoot = null;
    public Transform VisualsRoot => visualsRoot;

    protected override string DefaultInterationText => "Inspect";

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
        initialPosition = visualsRoot.localPosition;
        initialRotation = visualsRoot.localRotation;
        initialParent = visualsRoot.transform.parent;
    }

    public void RestoreVisualsRoot()
    {
        visualsRoot.transform.SetParent(initialParent, true);
        visualsRoot.transform.localPosition = initialPosition;
        visualsRoot.transform.localRotation = initialRotation;
    }
}
