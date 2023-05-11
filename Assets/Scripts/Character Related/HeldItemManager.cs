using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    Bounds projectedVisualsBounds = new Bounds();
    Vector3 projectedVisualsBoundsOffset;
    Snappable currentSnappable = null;

    public Pickupable HeldPickupable { get; private set; }
    bool IsDropMode => projectedVisuals != null;

    public void HoldItem(Pickupable pickupable, bool isInspecting)
    {
        HeldPickupable = pickupable;
        heldUseable = pickupable as Useable;
        HeldPickupable.HandlePickedUp();
        HeldPickupable.transform.SetParent(Camera.main.transform, true);
        if(isInspecting == false)
        {
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
    }

    public void MoveHeldItemToProperPosition()
    {
        HeldPickupable.transform.localPosition = holdLocalPosition;
        HeldPickupable.transform.localRotation = Quaternion.Euler(holdLocalRotation);
    }

    public void ActivateDropMode()
    {
        if(IsDropMode == false)
        {
            HeldPickupable.gameObject.SetActive(true);

            playerInput.HeldControlsEnabled = true;
            playerInput.InteractEnabled = true;
            //ClearProjectedVisuals();
            PlayerHUD.Instance.SetHeldScreenActive(true, true);
            projectedVisuals = HeldPickupable.GetVisualClone(HeldPickupable.transform.position, HeldPickupable.transform.rotation);
            Renderer[] projectedRenderers = projectedVisuals.GetComponentsInChildren<Renderer>();

            foreach(Renderer renderer in projectedRenderers)
            {
                //TODO Modify layers to not draw on special camera (we want to see it in world space)
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.material = dropIndicatorMaterial;
            }

            Collider[] boundsColliders = HeldPickupable.GetComponentsInChildren<Collider>();

            Quaternion heldPickupableCachedRotation = HeldPickupable.transform.rotation;
            HeldPickupable.transform.rotation = Quaternion.identity;
            Physics.SyncTransforms();

            projectedVisualsBounds.size = Vector3.zero;
            projectedVisualsBounds.center = boundsColliders[0].bounds.center;

            foreach(Collider collider in boundsColliders)
            {
                if(!collider.isTrigger)
                    projectedVisualsBounds.Encapsulate(collider.bounds);
            }

            projectedVisualsBoundsOffset = projectedVisualsBounds.center - HeldPickupable.transform.position;

            HeldPickupable.transform.rotation = heldPickupableCachedRotation;
        }
        else
            ReenterHeld();
    }

    protected void ClearProjectedVisuals()
    {
        if(projectedVisuals)
            Destroy(projectedVisuals);
        projectedVisuals = null;
    }

    public void ReenterHeld()
    {
        if(HeldPickupable)
        {
            HeldPickupable.gameObject.SetActive(true);

            if(IsDropMode)
                projectedVisuals.gameObject.SetActive(true);
            playerInput.HeldControlsEnabled = true;
            PlayerHUD.Instance.SetHeldScreenActive(true, IsDropMode);
        }
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

                ClearHeldItem();
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

    public void ClearHeldItem()
    {
        ClearProjectedVisuals();
        PlayerHUD.Instance.SetHeldScreenActive(false);
        HeldPickupable = null;
        heldUseable = null;
    }

    private void ProjectHeldItem()
    {
        RaycastHit triggerHitInfo;
        RaycastHit colliderHitInfo;

        Vector3 targetPosition;
        Quaternion targetRotation;

        Snappable snappable = null;
        bool triggerHit = Physics.BoxCast(Camera.main.transform.position, projectedVisualsBounds.extents, Camera.main.transform.forward, out triggerHitInfo, Quaternion.identity, dropRaycastDistance, dropRaycastMask, QueryTriggerInteraction.Collide);
        if(triggerHit)
        {
            snappable = triggerHitInfo.collider.GetComponent<Snappable>();
            if(snappable == false || snappable.AcceptsPickupable(HeldPickupable) == false)
            {//Make sure the trigger we hit was valid, otherwise, discard it
                triggerHit = false;
                snappable = null;
            }
        }
        RaycastHit hitInfoToUse = default;

        bool physicsHit = Physics.BoxCast(Camera.main.transform.position, projectedVisualsBounds.extents, Camera.main.transform.forward, out colliderHitInfo, Quaternion.identity, dropRaycastDistance, dropRaycastMask, QueryTriggerInteraction.Ignore);

        if(triggerHit || physicsHit)
        {
            if(triggerHit && physicsHit)
            {
                float colliderDistance = Vector3.Distance(colliderHitInfo.point, Camera.main.transform.position);
                float triggerDistance = Vector3.Distance(triggerHitInfo.point, Camera.main.transform.position);

                if(colliderDistance < triggerDistance)
                {
                    snappable = null;
                    hitInfoToUse = colliderHitInfo;
                }
                else
                    hitInfoToUse = triggerHitInfo;
            }
            else if(triggerHit)
            {
                hitInfoToUse = triggerHitInfo;
            }
            else if(physicsHit)
            {
                snappable = null;
                hitInfoToUse = colliderHitInfo;
            }

            Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + (Camera.main.transform.forward * hitInfoToUse.distance));

            if(snappable)
            {
                currentSnappable = snappable;
                targetPosition = snappable.SnapTransform.position;
                targetRotation = snappable.SnapTransform.rotation;
            }
            else
            {
                currentSnappable = null;
                targetPosition = Camera.main.transform.position + (Camera.main.transform.forward * hitInfoToUse.distance) - projectedVisualsBoundsOffset;
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

    public void HideProjectedVisualsAndControls()
    {
        if(HeldPickupable)
        {
            HeldPickupable.gameObject.SetActive(false);
            if(IsDropMode)
                projectedVisuals.gameObject.SetActive(false);
            playerInput.HeldControlsEnabled = false;
            PlayerHUD.Instance.SetHeldScreenActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        if (projectedVisuals != null)
        {
            Gizmos.DrawWireCube(projectedVisuals.transform.position + projectedVisualsBoundsOffset, projectedVisualsBounds.extents * 2f);
        }
    }
}
