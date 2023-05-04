using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldItemManager : MonoBehaviour
{
    [SerializeField] Material dropIndicatorMaterial;
    [SerializeField] PlayerInput playerInput;
    //TODO convert this to a distance, and a relative screen pos, to be consistent with different aspect ratios
    [SerializeField] Vector3 holdLocalPosition = Vector3.forward;
    [SerializeField] Vector3 holdLocalRotation = Vector3.zero;
    [SerializeField] float dropRaycastDistance = 5;
    [SerializeField] LayerMask dropRaycastMask;
    [SerializeField] ItemInspector itemInspector = null;
    [SerializeField] private float projectionSpeed = 1;

    Useable heldUseable = null;
    GameObject projectedVisuals = null;
    Snappable currentSnappable = null;

    public Pickupable HeldPickupable { get; private set; }
    bool IsDropMode => projectedVisuals != null;

    public void HoldItem(Pickupable pickupable)
    {
        HeldPickupable = pickupable;
        heldUseable = pickupable as Useable;
        HeldPickupable.HandlePickedUp();
        HeldPickupable.transform.SetParent(Camera.main.transform, true);
        MoveHeldItemToProperPosition();
        if(pickupable is Useable)
        {
            PlayerHUD.Instance.SetHeldScreenActive(true, false);
            playerInput.InteractEnabled = false;
            playerInput.HeldControlsEnabled = true;
        }
        else
            ActivateDropMode();
    }

    public void MoveHeldItemToProperPosition()
    {
        HeldPickupable.transform.localPosition = holdLocalPosition;
        HeldPickupable.transform.localRotation = Quaternion.Euler(holdLocalRotation);
    }

    public void ActivateDropMode()
    {
        playerInput.HeldControlsEnabled = true;
        playerInput.InteractEnabled = true;
        ClearProjectedVisuals();
        PlayerHUD.Instance.SetHeldScreenActive(true, true);
        //TODO is there a better way to clone the object? We really only want renderers getting colliders actually causes bugs
        projectedVisuals = Instantiate(HeldPickupable.VisualsPrefab);
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

    internal void ReenterHeld()
    {
        playerInput.HeldControlsEnabled = true;
        PlayerHUD.Instance.SetHeldScreenActive(true, IsDropMode);
    }

    private void Update()
    {
        if(HeldPickupable)
        {
            if(projectedVisuals)
                ProjectHeldItem();
            if(IsDropMode && playerInput.GetDropPressed())
            {
                //TODO only for visuals, not collisions until we're done maybe?
                if(currentSnappable)
                {
                    currentSnappable.SnapPickupable(HeldPickupable);
                    currentSnappable = null;
                    HeldPickupable.HandleDropped(false);
                }
                else
                {
                    HeldPickupable.transform.parent = null;
                    HeldPickupable.IsInteractable = true;
                    //TODO account for transform from visualRoot to parent
                    HeldPickupable.transform.position = projectedVisuals.transform.position;
                    HeldPickupable.transform.rotation = Quaternion.identity;
                    HeldPickupable.HandleDropped();
                }

                ClearProjectedVisuals();
                PlayerHUD.Instance.SetHeldScreenActive(false);
                HeldPickupable = null;
                heldUseable = null;
            }
            else if(itemInspector.IsInspecting == false && HeldPickupable != null && playerInput.GetInspectPressed())
            {
                PlayerHUD.Instance.SetHeldScreenActive(false);
                itemInspector.InspectItem(HeldPickupable);
            }
            else if(!IsDropMode && heldUseable && playerInput.GetUseButtonDown())
            {
                heldUseable.Use();
            }
        }
    }

    private void ProjectHeldItem()
    {
        RaycastHit hitInfo;
        Vector3 targetPosition;
        Quaternion targetRotation;

        //TODO Move to a bounds projection maybe?
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, dropRaycastDistance, dropRaycastMask))
        {
            Snappable snappable = hitInfo.collider.GetComponent<Snappable>();
            if(snappable && snappable.AcceptsPickupable(HeldPickupable))
            {
                currentSnappable = snappable;
                targetPosition = snappable.SnapTransform.position;
                targetRotation = snappable.SnapTransform.rotation;
            }
            else
            {
                currentSnappable = null;
                targetPosition = hitInfo.point;
                targetRotation = Quaternion.identity;
            }
        }
        else
        {
            currentSnappable = null;
            targetPosition = Camera.main.transform.position + Camera.main.transform.forward * dropRaycastDistance;
            targetRotation = Quaternion.identity;
        }
        projectedVisuals.transform.position = Vector3.Lerp(projectedVisuals.transform.position, targetPosition, Time.deltaTime * projectionSpeed);
        projectedVisuals.transform.rotation = Quaternion.Lerp(projectedVisuals.transform.rotation, targetRotation, Time.deltaTime * projectionSpeed);
    }
}
