using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInventoryElement : MonoBehaviour
{
    public Pickupable Pickupable { get; private set; }
    protected RectTransform rectTransform;

    public void Initialize(Pickupable pickupable)
    {
        Pickupable = pickupable;
        rectTransform = transform as RectTransform;
        Setup();
    }

    protected virtual void Setup()
    {

    }
}
