using UnityEngine;

///<summary>Interaction Goal for evaluating a Rotatable's snap point.</summary>
[RequireComponent(typeof(Rotatable))]
public class RotatableSnapGoal : InteractionGoal
{
    [SerializeField] private Rotatable rotatable;
    [SerializeField] private int snapGoal;

    private void OnValidate()
    {
        if (rotatable == null)
            rotatable = GetComponent<Rotatable>();

        snapGoal = (int)Mathf.Repeat(snapGoal, rotatable.AxisSnappingPositions);
    }

    private void Awake()
    {
        rotatable.OnFinishedRotationSnap.AddListener(HandleRotationSnapChanged);
    }

    private void HandleRotationSnapChanged(int snapResult)
    {
        IsComplete = snapResult == snapGoal;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = rotatable.TargetTransform.localToWorldMatrix;

        float angleRadian = snapGoal * Mathf.PI * 2f / (float)rotatable.AxisSnappingPositions;
        Vector3 snapPositionIndicatorOffset;

        if (rotatable.CurrentRotationAxis == Rotatable.RotationAxis.X)
            snapPositionIndicatorOffset = new Vector3(0, Mathf.Sin(angleRadian), Mathf.Cos(angleRadian));
        else if (rotatable.CurrentRotationAxis == Rotatable.RotationAxis.Y)
            snapPositionIndicatorOffset = new Vector3(Mathf.Cos(angleRadian), 0, Mathf.Sin(angleRadian));
        else
            snapPositionIndicatorOffset = new Vector3(Mathf.Cos(angleRadian), Mathf.Sin(angleRadian), 0);

        Gizmos.DrawWireSphere(snapPositionIndicatorOffset, .075f);

    }
}
