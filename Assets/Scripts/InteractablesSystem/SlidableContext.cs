using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidableContext : MonoBehaviour
{
    [SerializeField, Min(0)] private Vector2 bounds;
    [SerializeField] private bool snapEnabled;
    [SerializeField, Min(0)] private Vector2Int snapPoints;

    public bool SnapEnabled => snapEnabled;
    private EdgeCollider2D runtimeEdgeCollider;

    public class EndSlideResult
    {
        public bool snap;
        public Vector2Int snapIndex;
        public Vector3 localPosition;
        public Vector3 worldPosition;
    }

    internal class SnapAxisResult
    {
        public int snapIndex;
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
            slidable.transform.position = EndSlide(slidable.transform.position).worldPosition;
        }
    }

    /// <summary>
    /// Clamps the given position vector to be within the bounds of the SliderContext.
    /// </summary>
    /// <param name="position">The unclamped position.</param>
    /// <returns>The clamped position on the SliderContext's plane.</returns>
    public Vector3 ClampPosition(Vector3 position)
    {
        Vector3 relativePosition = transform.InverseTransformPoint(position);// Transform the position into the object's local space

        // Clamp the position within bounds
        relativePosition.x = Mathf.Clamp(relativePosition.x, -bounds.x, bounds.x);
        relativePosition.y = Mathf.Clamp(relativePosition.y, -bounds.y, bounds.y);
        relativePosition.z = 0;

        return transform.TransformPoint(relativePosition);// Result in World Space
    }

    /// <summary>
    /// Processes a slidables end position and provides resulting information after snap & local plane corrections. 
    /// </summary>
    /// <param name="position">The current position of the Slidable.</param>
    /// <returns>EndSlideResult with position & snap info.</returns>
    public EndSlideResult EndSlide(Vector3 position)
    {
        EndSlideResult result = new EndSlideResult();
        result.snap = snapEnabled;
        result.localPosition = transform.InverseTransformPoint(position);

        if (snapEnabled)
        {
            //get snap results for x and y axis
            if (snapPoints.x > 0)
            {
                SnapAxisResult xSnapResult = RoundToNearestSnapPoint(result.localPosition.x, bounds.x, snapPoints.x);
                result.snapIndex.x = xSnapResult.snapIndex;
                result.localPosition.x = xSnapResult.snapPosition;
            }

            if (snapPoints.y > 0)
            {
                SnapAxisResult ySnapResult = RoundToNearestSnapPoint(result.localPosition.y, bounds.y, snapPoints.y);
                result.snapIndex.y = ySnapResult.snapIndex;
                result.localPosition.y = ySnapResult.snapPosition;
            }
        }

        result.localPosition.z = 0;
        result.worldPosition = transform.TransformPoint(result.localPosition);
        return result;
    }

    /// <summary>
    /// Get the snap result of a specific axis.
    /// </summary>
    /// <param name="value">The current relative position on the corresponding axis.</param>
    /// <param name="bounds">The contexts bounds of the corresponding axis.</param>
    /// <param name="pointCount">The contexts snap point count of the corresponding axis.</param>
    /// <returns>SnapAxisResult with  resulting local position & snap index.</returns>
    internal static SnapAxisResult RoundToNearestSnapPoint(float value, float bounds, int pointCount)
    {
        SnapAxisResult result = new SnapAxisResult();

        if (pointCount == 1)
        {
            result.snapIndex = 0;
            result.snapPosition = 0; //snap to center
            return result;
        }

        float snapDelta = 2 * bounds / (pointCount - 1); // Calculate the size of each snap point
        result.snapIndex = Mathf.RoundToInt((value + bounds) / snapDelta); // Calculate the index of the nearest snap point      
        result.snapPosition = result.snapIndex * snapDelta - bounds; // Calculate the position of of the nearest snap point
        return result;
    }

    void OnDrawGizmos()
    {
        Vector3 corner1 = transform.TransformPoint(new Vector3(-bounds.x, bounds.y));
        Vector3 corner2 = transform.TransformPoint(new Vector3(bounds.x, bounds.y));
        Vector3 corner3 = transform.TransformPoint(new Vector3(bounds.x, -bounds.y));
        Vector3 corner4 = transform.TransformPoint(new Vector3(-bounds.x, -bounds.y));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner4);
        Gizmos.DrawLine(corner4, corner1);

        if (snapEnabled)
        {
            Gizmos.color = Color.red;
            int virtualSnapPointsX = snapPoints.x > 1 ? snapPoints.x - 1 : 1;
            int virtualSnapPointsY = snapPoints.y > 1 ? snapPoints.y - 1 : 1;

            for (int x = 0; x < virtualSnapPointsX + 1; x++)
            {
                for (int y = 0; y < virtualSnapPointsY + 1; y++)
                {
                    float xOffset = virtualSnapPointsX > 1 ? (x * ((bounds.x * 2) / virtualSnapPointsX)) - bounds.x : 0;
                    float yOffset = virtualSnapPointsY > 1 ? (y * ((bounds.y * 2) / virtualSnapPointsY)) - bounds.y : 0;
                    Gizmos.DrawSphere(transform.TransformPoint(new Vector3(xOffset, yOffset, 0)), .0025f);
                }
            }
        }

        Gizmos.color = Color.white;
    }
}
