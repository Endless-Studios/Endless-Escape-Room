using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickupable : Inspectable
{
    internal void HandleDropped()
    {
        CacheTransform();
    }
}
