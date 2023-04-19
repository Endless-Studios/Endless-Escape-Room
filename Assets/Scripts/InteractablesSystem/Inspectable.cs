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

    private void Awake()
    {
        CacheTransform();
    }

    protected void CacheTransform()
    {
        initialRotation = visualsRoot.rotation;
        initialPosition = visualsRoot.position;
        initialParent = visualsRoot.transform.parent;
    }

    public void RestoreVisualsRoot()
    {
        visualsRoot.transform.position = initialPosition;
        visualsRoot.transform.rotation = initialRotation;
        visualsRoot.transform.SetParent(initialParent, true);
    }
}
