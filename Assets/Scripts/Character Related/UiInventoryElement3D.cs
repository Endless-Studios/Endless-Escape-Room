using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UiInventoryElement3D : UiInventoryElement
{
    [SerializeField] float depth = 1;
    GameObject visuals = null;

    Bounds finalBounds;

    protected override void Setup()
    {
        visuals = Pickupable.GetVisualClone(Pickupable.transform.position, Quaternion.identity);
        visuals.transform.SetParent(Camera.main.transform, true);
        PositionVisuals();
        AdjustSize();
        Unhighlight();
    }

    void AdjustSize() //TODO this is slightly innacurate depending on rotation
    {
        Bounds[] bounds = visuals.GetComponentsInChildren<Renderer>().Select(r => r.bounds).ToArray();
        finalBounds = bounds[0];
        //Debug.Log(bounds.Length);
        for(int index = 1; index < bounds.Length; index++)
        {
            finalBounds.Encapsulate(bounds[index]);
        }
        //Debug.Log(finalBounds.center);

        Vector3 center = finalBounds.center;
        Vector3 extents = finalBounds.extents;
        Vector2[] extentPoints = new Vector2[8]
        {
            Camera.main.WorldToScreenPoint(new Vector3(center.x - extents.x, center.y - extents.y, center.z - extents.z)),
            Camera.main.WorldToScreenPoint(new Vector3(center.x + extents.x, center.y - extents.y, center.z - extents.z)),
            Camera.main.WorldToScreenPoint(new Vector3(center.x - extents.x, center.y - extents.y, center.z + extents.z)),
            Camera.main.WorldToScreenPoint(new Vector3(center.x + extents.x, center.y - extents.y, center.z + extents.z)),
            Camera.main.WorldToScreenPoint(new Vector3(center.x - extents.x, center.y + extents.y, center.z - extents.z)),
            Camera.main.WorldToScreenPoint(new Vector3(center.x + extents.x, center.y + extents.y, center.z - extents.z)),
            Camera.main.WorldToScreenPoint(new Vector3(center.x - extents.x, center.y + extents.y, center.z + extents.z)),
            Camera.main.WorldToScreenPoint(new Vector3(center.x + extents.x, center.y + extents.y, center.z + extents.z))
        };

        Vector2 min = extentPoints[0];
        Vector2 max = extentPoints[0];

        foreach(Vector2 extentPoint in extentPoints)
        {
            min = Vector2.Min(min, extentPoint);
            max = Vector2.Max(max, extentPoint);
        }

        //Rect screenRect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
        float width = max.x - min.x;
        float height = max.y - min.y;
        float largestDimension = Mathf.Max(width, height);
        float scale = rectTransform.sizeDelta.x * PlayerHUD.Instance.transform.localScale.x / largestDimension;
        visuals.transform.localScale *= scale;
    }

    private void LateUpdate()
    {
        PositionVisuals();
    }

    private void PositionVisuals()
    {
        visuals.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(rectTransform.position.x, rectTransform.position.y, depth));
    }

    private void OnDestroy()
    {
        Destroy(visuals);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.matrix = visuals.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(finalBounds.center, finalBounds.size);
    }

    public override void Highlight()
    {
        Interactable.SetLayerRecursive(visuals.transform, LayerMask.NameToLayer("InspectedOutline"));//TODO centralize layer management functions, and convert to new layer, on new camera in stack.
    }

    public override void Unhighlight()
    {
        Interactable.SetLayerRecursive(visuals.transform, LayerMask.NameToLayer("InspectedItem"));//TODO centralize layer management functions, and convert to new layer, on new camera in stack.
    }
}
