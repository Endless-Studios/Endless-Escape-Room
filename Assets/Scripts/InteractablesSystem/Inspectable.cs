using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inspectable : Interactable
{
    [SerializeField] Transform visualsRoot = null;
    public Transform VisualsRoot => visualsRoot;

    Quaternion initialRotation;
    Vector3 initialPosition;
    Transform initialParent;

    const string inspectedLayer = "InspectedItem";

    private void Awake()
    {
        CacheTransform();
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

    public void SetToHeldLayer()
    {
        SetLayerRecursive(transform, LayerMask.NameToLayer(inspectedLayer));
    }
}
