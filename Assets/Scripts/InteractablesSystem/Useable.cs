using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Useable : Pickupable
{
    public UnityEvent OnUsed = new UnityEvent();

    public void Use()
    {
        OnUsed.Invoke();
    }
}
