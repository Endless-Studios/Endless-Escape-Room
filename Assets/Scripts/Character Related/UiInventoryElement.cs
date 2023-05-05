using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiInventoryElement : MonoBehaviour
{
    public Pickupable Pickupable { get; private set; }

    public void Initialize(Pickupable pickupable)
    {
        Pickupable = pickupable;
        Setup();
    }

    protected virtual void Setup()
    {

    }
}
