using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidableContext : MonoBehaviour
{
    [Header("Slidable Context")]
    [SerializeField] protected bool keepSlidablesOnZPlane = true;

    public class EndSlideResult
    {
        public bool snap;
        public Vector2Int snapID;
        public Vector3 localPosition;
        public Vector3 worldPosition;
    }

    /// <summary>
    /// If no context is provided, this default plane can be used. 
    /// </summary>
    public static Plane GetDefaultPlaneAtPosition(Vector3 targetPosition)
    {
        return new Plane(Vector3.up, targetPosition);
    }

    private void Awake()
    {
        SnapAllSlidables();
    }

    [ContextMenu("Snap All Children")] // Can be called from context menu in editor to fix position in scene
    protected void SnapAllSlidables()
    {
        Slidable[] allslidables = gameObject.GetComponentsInChildren<Slidable>();

        foreach (Slidable slidable in allslidables)
        {
            slidable.transform.position = GetEndSlideResult(slidable.transform.position).worldPosition;
        }
    }

    /// <summary>
    /// Get the context's Plane for raycasting against.
    /// </summary>
    /// <param name="targetSlidable">The slidable requesting the context's Plane.</param>
    /// <returns>The context's Plane.</returns>
    public Plane GetContextPlane(Slidable targetSlidable)
    {
        if (keepSlidablesOnZPlane)
            return new Plane(transform.forward, transform.position);
        else
            return new Plane(transform.forward, targetSlidable.transform.position);
    }

    /// <summary>
    /// Clamps the given position vector to be within the boundary of the SliderContext.
    /// </summary>
    /// <param name="currentWorldPosition">The current unclamped world position.</param>
    /// <returns>The clamped position on the SliderContext's plane in world space.</returns>
    public virtual Vector3 ClampPosition(Vector3 currentWorldPosition)
    {
        Vector3 relativePosition = transform.InverseTransformPoint(currentWorldPosition);// Transform the position into the object's local space

        if (keepSlidablesOnZPlane)
            relativePosition.z = 0;

        return transform.TransformPoint(relativePosition);// Result in World Space
    }

    /// <summary>
    /// Processes a slidables end position and provides resulting information after snap & local plane corrections. 
    /// </summary>
    /// <param name="currentWorldPosition">The current position of the Slidable.</param>
    /// <returns>EndSlideResult with position & snap info.</returns>
    public virtual EndSlideResult GetEndSlideResult(Vector3 currentWorldPosition)
    {
        EndSlideResult result = new EndSlideResult();
        result.snap = false;
        result.localPosition = transform.InverseTransformPoint(currentWorldPosition);

        if (keepSlidablesOnZPlane)
            result.localPosition.z = 0;

        result.worldPosition = transform.TransformPoint(result.localPosition);
        return result;
    }

    public virtual void DrawGizmosForSelectedSlidable(Slidable slidable)
    {
        DrawGizmosPlaneAtPosition(slidable.transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        DrawGizmosPlaneAtPosition(transform.position);
    }

    private void DrawGizmosPlaneAtPosition(Vector3 targetPosition)
    {
        Gizmos.matrix = Matrix4x4.TRS(targetPosition, Quaternion.LookRotation(transform.up, transform.forward), Vector3.one);
        Vector3 extents = new Vector3(1f, .0025f, 1f);
        Color drawColor = Color.green;
        drawColor.a = .2f;
        Gizmos.color = drawColor;
        Gizmos.DrawCube(Vector3.zero, extents);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, extents);
        Gizmos.color = Color.white;
    }
}
