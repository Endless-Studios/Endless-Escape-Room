using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Slidable))]
public class SlidableFreeGoal : InteractionGoal
{
    [SerializeField] private Slidable slidable;
    [SerializeField] private Vector2 goalPosition;
    [SerializeField, Min(0)] private float distanceTolerance = .1f;

    private void OnValidate()
    {
        if (slidable == null)
            slidable = GetComponent<Slidable>();
    }

    private void Awake()
    {
        slidable.OnPositionMoved.AddListener(HandleSlidablePositionMoved);
    }

    private void HandleSlidablePositionMoved(Vector2 movePosition)
    {
        if (Vector2.Distance(movePosition, goalPosition) <= distanceTolerance)
            IsComplete = true;
        else
            IsComplete = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, .3f);

        Vector3 goalWorldPosition;
        Vector3 contextNormal;

        if (slidable.SlidableContext != null)
        {
            goalWorldPosition = slidable.SlidableContext.transform.TransformPoint(goalPosition);
            contextNormal = slidable.SlidableContext.transform.forward;
        }
        else
        {
            goalWorldPosition = new Vector3(goalPosition.x, 0, goalPosition.y);
            contextNormal = Vector3.up;
        }
 
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(goalWorldPosition, contextNormal, distanceTolerance);
#endif
    }
}