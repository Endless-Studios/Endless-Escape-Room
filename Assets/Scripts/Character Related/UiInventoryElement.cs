using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UiInventoryElement : MonoBehaviour
{
    public Pickupable Pickupable { get; private set; }
    protected RectTransform rectTransform;

    public void Initialize(Pickupable pickupable)
    {
        Pickupable = pickupable;
        rectTransform = transform as RectTransform;
        Setup();
    }

    protected abstract void Setup();
    public abstract void Highlight();
    public abstract void Unhighlight();
}
