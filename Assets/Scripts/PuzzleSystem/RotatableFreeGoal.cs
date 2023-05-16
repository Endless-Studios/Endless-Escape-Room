using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rotatable))]
public class RotatableFreeGoal : InteractionGoal
{
    [SerializeField] private Rotatable rotatable;
    [SerializeField] private float goalRotation;
    [SerializeField, Range(0, 180)] private float rotationTolerance = 5f;

    private void OnValidate()
    {
        if (rotatable == null)
            rotatable = GetComponent<Rotatable>();

        goalRotation = Mathf.Repeat(goalRotation, 360);
    }

    private void Awake()
    {
        rotatable.OnFinishedRotation.AddListener(HandleFinishedRotation);
    }

    private void HandleFinishedRotation(float rotationResult)
    {
        if (Mathf.Abs(Mathf.DeltaAngle(goalRotation, rotationResult)) <= rotationTolerance)
            IsComplete = true;
        else
            IsComplete = false;
    }

    void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.yellow / 3f;
        UnityEditor.Handles.matrix = rotatable.TargetTransform.localToWorldMatrix;

        Vector3 forwardVector = Vector3.forward;
        Vector3 normalVector = Vector3.right;

        if (rotatable.CurrentRotationAxis == Rotatable.RotationAxis.Y)
        {
            forwardVector = Vector3.right;
            normalVector = Vector3.up;
        }
        else if (rotatable.CurrentRotationAxis == Rotatable.RotationAxis.Z)
        {
            forwardVector = Vector3.right;
            normalVector = Vector3.forward;
        }

        UnityEditor.Handles.DrawSolidArc(Vector3.zero, normalVector, Quaternion.AngleAxis(goalRotation + rotationTolerance, -normalVector) * forwardVector, rotationTolerance * 2f, 1f);
#endif
    }
}
