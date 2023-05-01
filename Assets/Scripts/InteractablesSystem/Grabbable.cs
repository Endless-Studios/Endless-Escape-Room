using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : Interactable
{
    protected override string DefaultInterationText => "Grab";

    internal void StopInteract()
    {
    }
}
