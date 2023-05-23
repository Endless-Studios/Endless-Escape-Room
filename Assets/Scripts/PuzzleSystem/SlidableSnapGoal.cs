using UnityEngine;

/// <summary>
/// Interaction goal for evaluating a Slidable's snap point.
/// </summary>
public class SlidableSnapGoal : InteractionGoal
{
    private enum SnapEvaluation { BothXY, OnlyX, OnlyY }
    [SerializeField] private Slidable slidable;
    [SerializeField, Min(0)] private Vector2Int snapGoal = new Vector2Int();
    [SerializeField] private SnapEvaluation snapEvaluation;

    private void OnValidate()
    {
        if (slidable == null)
            slidable = GetComponent<Slidable>();
    }

    private void Awake()
    {
        if (slidable == null)
        {
            Debug.LogWarning("Slidable missing from goal, removing goal.");
            GameObject.Destroy(this);
        }
        else
            slidable.OnPositionSnapped.AddListener(HandleSlidableSnapChanged);
    }

    private void OnDestroy()
    {
        if (slidable != null)
            slidable.OnPositionSnapped.RemoveListener(HandleSlidableSnapChanged);
    }

    private void HandleSlidableSnapChanged(Vector2Int snapValue)
    {
        if (snapEvaluation == SnapEvaluation.BothXY && snapValue == snapGoal)
            IsComplete = true;
        else if (snapEvaluation == SnapEvaluation.OnlyX && snapValue.x == snapGoal.x)
            IsComplete = true;
        else if (snapEvaluation == SnapEvaluation.OnlyY && snapValue.y == snapGoal.y)
            IsComplete = true;
        else
            IsComplete = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (slidable.SlidableContext == null)
            return;

        SlidableContextWithBoundarySnappable snappableContext = (SlidableContextWithBoundarySnappable)slidable.SlidableContext;

        if (snappableContext == null)
            return;

        Gizmos.matrix = snappableContext.transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        Vector2 extents = snappableContext.Size / 2f;
        float gizmosSphereSize = (extents.x + extents.y) / 15f;

        float snapDistanceX = snappableContext.SnapPoints.x > 1 ? snappableContext.Size.x / (snappableContext.SnapPoints.x - 1) : 0;
        float snapDistanceY = snappableContext.SnapPoints.y > 1 ? snappableContext.Size.y / (snappableContext.SnapPoints.y - 1) : 0;

        if (snapEvaluation == SnapEvaluation.BothXY)
        {
            Gizmos.DrawWireSphere(new Vector3((snapDistanceX * snapGoal.x) - extents.x, (snapDistanceY * snapGoal.y) - extents.y, 0), gizmosSphereSize);
        }
        else if (snapEvaluation == SnapEvaluation.OnlyX && snappableContext.SnapPoints.x > 0)
        {
            if (snappableContext.SnapPoints.x == 1)
                extents.x = 0;

            Gizmos.DrawWireCube(new Vector3((snapDistanceX * snapGoal.x) - extents.x, 0, 0), new Vector3(gizmosSphereSize, extents.y * 2, gizmosSphereSize));
        }
        else if (snapEvaluation == SnapEvaluation.OnlyY && snappableContext.SnapPoints.y > 0)
        {
            if (snappableContext.SnapPoints.y == 1)
                extents.y = 0;

            Gizmos.DrawWireCube(new Vector3(0, (snapDistanceY * snapGoal.y) - extents.y, 0), new Vector3(extents.x * 2, gizmosSphereSize, gizmosSphereSize));
        }
    }
}
