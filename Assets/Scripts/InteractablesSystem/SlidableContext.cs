using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidableContext : MonoBehaviour
{
    [SerializeField, Min(0)] private Vector2 size;
    [SerializeField] private bool snapEnabled;
    [SerializeField, Min(0)] private Vector2Int snapPoints;

    public bool SnapEnabled => snapEnabled;
    private EdgeCollider2D runtimeEdgeCollider;

    public class EndSlideResult
    {
        public bool snap;
        public Vector2Int snapID;
        public Vector3 localPosition;
        public Vector3 worldPosition;
    }

    private class SnapAxisResult
    {
        public int snapID;
        public float snapPosition;
    }

    private void Awake()
    {
        SnapAllSlidables();
    }

    [ContextMenu("Snap All Children")] // Can be called from context menu in editor to fix position in scene
    private void SnapAllSlidables()
    {
        Slidable[] allslidables = gameObject.GetComponentsInChildren<Slidable>();

        foreach (Slidable slidable in allslidables)
        {
            slidable.transform.position = GetEndSlideResult(slidable.transform.position).worldPosition;
        }
    }

    /// <summary>
    /// Clamps the given position vector to be within the boundary of the SliderContext.
    /// </summary>
    /// <param name="currentWorldPosition">The current unclamped world position.</param>
    /// <returns>The clamped position on the SliderContext's plane in world space.</returns>
    public Vector3 ClampPosition(Vector3 currentWorldPosition)
    {
        Vector3 relativePosition = transform.InverseTransformPoint(currentWorldPosition);// Transform the position into the object's local space

        // Clamp the position within boundary
        relativePosition.x = Mathf.Clamp(relativePosition.x, -size.x / 2f, size.x / 2f);
        relativePosition.y = Mathf.Clamp(relativePosition.y, -size.y / 2f, size.y / 2f);
        relativePosition.z = 0;

        return transform.TransformPoint(relativePosition);// Result in World Space
    }

    /// <summary>
    /// Processes a slidables end position and provides resulting information after snap & local plane corrections. 
    /// </summary>
    /// <param name="currentWorldPosition">The current position of the Slidable.</param>
    /// <returns>EndSlideResult with position & snap info.</returns>
    public EndSlideResult GetEndSlideResult(Vector3 currentWorldPosition)
    {
        EndSlideResult result = new EndSlideResult();
        result.snap = snapEnabled;
        result.localPosition = transform.InverseTransformPoint(currentWorldPosition);

        if (snapEnabled)
        {
            //get snap results for x and y axis
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
        }

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
    private static SnapAxisResult GetSnapAxisResult(float currentLocalPosition, float axisSize, int pointCount)
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

    void OnDrawGizmos()
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

        if (snapEnabled && (snapPoints.x > 0 || snapPoints.y > 0))
        {
            Gizmos.color = Color.red;
            float gizmosSphereSize = (extents.x + extents.y) / 20f;
            int virtualSnapPointsX = snapPoints.x > 1 ? snapPoints.x - 1 : 1;
            int virtualSnapPointsY = snapPoints.y > 1 ? snapPoints.y - 1 : 1;

            for (int x = 0; x < virtualSnapPointsX + 1; x++)
            {
                for (int y = 0; y < virtualSnapPointsY + 1; y++)
                {
                    float xOffset = snapPoints.x > 1 ? (x * ((size.x) / virtualSnapPointsX)) - extents.x : 0;
                    float yOffset = snapPoints.y > 1 ? (y * ((size.y) / virtualSnapPointsY)) - extents.y : 0;

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
