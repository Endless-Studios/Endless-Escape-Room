using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryBase : MonoBehaviour
{
    [SerializeField] Material dropIndicatorMaterial;

    public abstract Pickupable HeldItem { get; }

    public abstract bool CanPickupItem(Pickupable pickupable);
    public abstract bool PickupItem(Pickupable pickupable);

    protected Pickupable heldPickupable = null;
    protected Useable heldUseable = null;
    protected GameObject projectedVisuals = null;

    protected bool IsDropMode => projectedVisuals != null;

    public void ActivateDropMode()
    {
        ClearProjectedVisuals();
        PlayerHUD.Instance.SetHeldScreenActive(true, true);
        //TODO is there a better way to clone the object? We really only want renderers
        projectedVisuals = Instantiate(heldPickupable.VisualsRoot.gameObject);
        Renderer[] projectedRenderers = projectedVisuals.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in projectedRenderers)
        {
            //TODO Modify layers to not draw on special camera (we want to see it in world space)
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.material = dropIndicatorMaterial;
        }
    }

    protected void ClearProjectedVisuals()
    {
        if(projectedVisuals)
            Destroy(projectedVisuals);
        projectedVisuals = null;
    }

    internal void ReenterInspect()
    {
        PlayerHUD.Instance.SetHeldScreenActive(true, false);
    }
}
