using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Slidable))]
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
        slidable.OnPositionSnapped.AddListener(HandleSlidableSnapChanged);
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

        if (snapEvaluation == SnapEvaluation.BothXY)
        {
            Vector2 extents = snappableContext.Size / 2f;
            float gizmosSphereSize = (extents.x + extents.y) / 15f;
            float snapDistanceX = snappableContext.SnapPoints.x > 1 ? snappableContext.Size.x / (snappableContext.SnapPoints.x - 1) : 0;
            float snapDistanceY = snappableContext.SnapPoints.y > 1 ? snappableContext.Size.y / (snappableContext.SnapPoints.y - 1) : 0;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3((snapDistanceX * snapGoal.x) - extents.x, (snapDistanceY * snapGoal.y) - extents.y, 0), gizmosSphereSize);
        }
        //box gizmos support
    }
}
