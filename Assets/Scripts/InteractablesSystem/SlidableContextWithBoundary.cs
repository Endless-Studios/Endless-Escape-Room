using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidableContextWithBoundary : SlidableContext
{
    [Header("Boundary")]
    [SerializeField, Min(0)] protected Vector2 size = Vector2.one;

    public override Vector3 ClampPosition(Vector3 currentWorldPosition)
    {
        Vector3 relativePosition = transform.InverseTransformPoint(currentWorldPosition);// Transform the position into the object's local space

        // Clamp the position within boundary
        relativePosition.x = Mathf.Clamp(relativePosition.x, -size.x / 2f, size.x / 2f);
        relativePosition.y = Mathf.Clamp(relativePosition.y, -size.y / 2f, size.y / 2f);

        if (keepSlidablesOnZPlane)
            relativePosition.z = 0;

        return transform.TransformPoint(relativePosition);// Result in World Space
    }

    public override void DrawGizmosForSelectedSlidable(Slidable targetSlidable)
    {
        //left blank to override base behavior
    } 

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Vector2 extents = size / 2f;

        Vector3 corner1 = new Vector3(-extents.x, extents.y);
        Vector3 corner2 = new Vector3(extents.x, extents.y);
        Vector3 corner3 = new Vector3(extents.x, -extents.y);
        Vector3 corner4 = new Vector3(-extents.x, -extents.y);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner4);
        Gizmos.DrawLine(corner4, corner1);

        if (keepSlidablesOnZPlane == false)
        {
            Vector3 depthOffset = new Vector3(0, 0, (extents.x + extents.y) / 2f);
            Gizmos.DrawLine(corner1 + depthOffset, corner1 - depthOffset);
            Gizmos.DrawLine(corner2 + depthOffset, corner2 - depthOffset);
            Gizmos.DrawLine(corner3 + depthOffset, corner3 - depthOffset);
            Gizmos.DrawLine(corner4 + depthOffset, corner4 - depthOffset);
        }

        Gizmos.color = Color.white;
    }
}
