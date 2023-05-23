using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidableContextWithBoundarySnappable : SlidableContextWithBoundary
{
    [Header("Snapping")]
    [SerializeField, Min(0)] private Vector2Int snapPoints = new Vector2Int(2, 2);

    public Vector2Int SnapPoints => snapPoints;

    protected class SnapAxisResult
    {
        public int snapID;
        public float snapPosition;
    }

    public override EndSlideResult GetEndSlideResult(Vector3 currentWorldPosition)
    {
        EndSlideResult result = new EndSlideResult();

        result.localPosition = transform.InverseTransformPoint(currentWorldPosition);

        result.snap = true;

        if (snapPoints.x > 0)
        {
            SnapAxisResult xSnapResult = GetSnapAxisResult(result.localPosition.x, size.x, snapPoints.x);
            result.snapID.x = xSnapResult.snapID;
            result.localPosition.x = xSnapResult.snapPosition;
        }

        if (snapPoints.y > 0)
        {
            SnapAxisResult ySnapResult = GetSnapAxisResult(result.localPosition.y, size.y, snapPoints.y);
            result.snapID.y = ySnapResult.snapID;
            result.localPosition.y = ySnapResult.snapPosition;
        }

        if (keepSlidablesOnZPlane)
            result.localPosition.z = 0;

        result.worldPosition = transform.TransformPoint(result.localPosition);
        return result;
    }

    /// <summary>
    /// Get the snap result by rounding to the nearest snap point of a specific axis.
    /// </summary>
    /// <param name="currentLocalPosition">The current relative position on the corresponding axis.</param>
    /// <param name="axisSize">The contexts boundary of the corresponding axis.</param>
    /// <param name="pointCount">The contexts snap point count of the corresponding axis.</param>
    /// <returns>SnapAxisResult with  resulting local position & snap index.</returns>
    protected static SnapAxisResult GetSnapAxisResult(float currentLocalPosition, float axisSize, int pointCount)
    {
        SnapAxisResult result = new SnapAxisResult();

        if (pointCount == 1)
        {
            result.snapID = 0;
            result.snapPosition = 0; //snap to center
            return result;
        }

        float snapDistance = axisSize / (pointCount - 1); // Calculate the size of each snap point
        result.snapID = Mathf.RoundToInt((currentLocalPosition + axisSize / 2f) / snapDistance); // Calculate the index of the nearest snap point      
        result.snapPosition = result.snapID * snapDistance - axisSize / 2f; // Calculate the position of of the nearest snap point
        return result;
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

        if (snapPoints.x > 0 || snapPoints.y > 0)
        {
            Gizmos.color = new Color(0, 1, 1,.8f);
            float gizmosSphereSize = (extents.x + extents.y) / 20f;
            int virtualSnapPointsX = snapPoints.x > 1 ? snapPoints.x - 1 : 0;
            int virtualSnapPointsY = snapPoints.y > 1 ? snapPoints.y - 1 : 0;

            for (int x = 0; x < virtualSnapPointsX + 1; x++)
            {
                for (int y = 0; y < virtualSnapPointsY + 1; y++)
                {
                    float xOffset = snapPoints.x > 1 ? (x * ((size.x) / virtualSnapPointsX)) - extents.x : 0;
                    float yOffset = snapPoints.y > 1 ? (y * ((size.y) / virtualSnapPointsY)) - extents.y : 0;

#if UNITY_EDITOR
                    UnityEditor.Handles.matrix = Gizmos.matrix;
                    UnityEditor.Handles.Label(new Vector3(xOffset, yOffset, 0), $"({x},{y})");
#endif

                    if (snapPoints.x == 0)
                    {
                        Gizmos.DrawCube(new Vector3(xOffset, yOffset, 0), new Vector3(size.x, gizmosSphereSize, gizmosSphereSize));
                    }
                    else if (snapPoints.y == 0)
                    {
                        Gizmos.DrawCube(new Vector3(xOffset, yOffset, 0), new Vector3(gizmosSphereSize, size.y, gizmosSphereSize));
                    }
                    else
                    {
                        Gizmos.DrawSphere(new Vector3(xOffset, yOffset, 0), gizmosSphereSize);
                    }
                }
            }
        }

        Gizmos.color = Color.white;
    }
}
