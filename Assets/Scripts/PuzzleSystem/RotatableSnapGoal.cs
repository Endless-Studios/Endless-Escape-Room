using UnityEngine;

/// <summary>
/// Interaction Goal for evaluating a Rotatable's snap point.
/// </summary>
public class RotatableSnapGoal : InteractionGoal
{
    [SerializeField] private Rotatable rotatable;
    [SerializeField] private int snapGoal;

    private void OnValidate()
    {
        if (rotatable == null)
            rotatable = GetComponent<Rotatable>();

        //Keep snap goal within valid range
        snapGoal = (int)Mathf.Repeat(snapGoal, rotatable.AxisSnappingPositions);
    }

    private void Awake()
    {
        if (rotatable == null)
        {
            Debug.LogWarning("Rotatable missing from goal, removing goal.");
            GameObject.Destroy(this);
        }
        else
            rotatable.OnFinishedRotationSnap.AddListener(HandleRotationSnapChanged);
    }

    private void OnDestroy()
    {
        if (rotatable != null)
            rotatable.OnFinishedRotationSnap.RemoveListener(HandleRotationSnapChanged);
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
