using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderContext : MonoBehaviour
{
    [SerializeField, Min(0)] private Vector2 bounds;
    [SerializeField] private bool snapEnabled;
    [SerializeField, Min(0)] private Vector2Int snapPoints;

    public bool SnapEnabled => snapEnabled;
    private EdgeCollider2D runtimeEdgeCollider;

    private void Awake()
    {
        SnapAllSlidables();
    }

    public Vector3 ClampPosition(Vector3 position)
    {
        Vector3 relativePosition = transform.InverseTransformPoint(position);
        relativePosition.x = Mathf.Clamp(relativePosition.x, -bounds.x, bounds.x);
        relativePosition.y = Mathf.Clamp(relativePosition.y, -bounds.y, bounds.y);
        // relativePosition.z = 0;
        return transform.TransformPoint(relativePosition);
    }

    public Vector3 SnapPosition(Vector3 position)
    {
        Vector3 relativePosition = transform.InverseTransformPoint(position);
        Vector3 snapOffset = relativePosition;

        if (snapEnabled)
        {
            if (snapPoints.x > 0)
                snapOffset.x = RoundToNearestSnapPoint(relativePosition.x, bounds.x, snapPoints.x);

            if (snapPoints.y > 0)
                snapOffset.y = RoundToNearestSnapPoint(relativePosition.y, bounds.y, snapPoints.y);
        }

        snapOffset.z = 0;

        return transform.TransformPoint(snapOffset);
    }

    internal static float RoundToNearestSnapPoint(float value, float bounds, int pointCount)
    {
        if (pointCount == 1)
            return 0;

        float snapSize = 2 * bounds / (pointCount - 1); // Calculate the size of each snap point
        int snapIndex = Mathf.RoundToInt((value + bounds) / snapSize); // Calculate the index of the nearest snap point
        // Debug.Log("SnapIndex: " + snapIndex);
        float snappedValue = snapIndex * snapSize - bounds; // Calculate the value of the nearest snap point
        return snappedValue;
    }

    [ContextMenu("Snap All Children")] // Can be called from context menu in editor
    private void SnapAllSlidables()
    {
        Slidable[] allslidables = gameObject.GetComponentsInChildren<Slidable>();

        foreach (Slidable slidable in allslidables)
        {
            slidable.transform.position = SnapPosition(slidable.transform.position);
        }
    }

    void OnDrawGizmos()//Selected
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
        Gizmos.color = Color.red;

        if (snapEnabled)
        {
            int virtualSnapPointsX = snapPoints.x > 1 ? snapPoints.x - 1 : 0;
            int virtualSnapPointsY = snapPoints.y > 1 ? snapPoints.y - 1 : 0;

            for (int x = 0; x < virtualSnapPointsX + 1; x++)
            {
                for (int y = 0; y < virtualSnapPointsY + 1; y++)
                {
                    float xOffset = (x * ((bounds.x * 2) / virtualSnapPointsX)) - bounds.x;
                    float yOffset = (y * ((bounds.y * 2) / virtualSnapPointsY)) - bounds.y;
                    Gizmos.DrawSphere(transform.TransformPoint(new Vector3(xOffset, yOffset, 0)), .0025f);
                }
            }
        }

        Gizmos.color = Color.white;
    }
}
