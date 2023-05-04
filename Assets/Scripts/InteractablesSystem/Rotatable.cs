using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotatable : Grabbable
{
    public enum RotationAxis { X, Y, Z }
    const float SMOOTH_SNAP_TIME = .1f;
    //Event: rotation finished float
    //Event: rotation finished snap index

    protected override string DefaultInteractionText => "Rotate";

    [SerializeField] private Transform targetTransform;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private RotationAxis rotationAxis;
    [SerializeField, Min(0)] private int axisSnappingPositions = 0;

    private Coroutine activeSnapCoroutine;
    private Quaternion startingRotation;

    private Vector3 rotationAxisVector
    {
        get
        {
            if (rotationAxis == RotationAxis.X) return Vector3.right;
            else if (rotationAxis == RotationAxis.Y) return Vector3.up;
            else return Vector3.forward;
        }
    }

    private void Awake()
    {
        if (targetTransform == null)
            targetTransform = transform;

        startingRotation = targetTransform.rotation;        
    }

    protected override void InternalHandleInteract()
    {
        base.InternalHandleInteract();

        if (activeSnapCoroutine != null)
        {
            StopCoroutine(activeSnapCoroutine);
            activeSnapCoroutine = null;
        }
    }

    protected override void HandleStopInteract()
    {
        if (axisSnappingPositions > 0)
        {
            Quaternion targetRotation = CalculateNearestSnapAngle();
            activeSnapCoroutine = StartCoroutine(SmoothToRotation(targetRotation));
        }
    }

    public override void HandleUpdate()
    {
        Vector2 mouseInput = PlayerCore.LocalPlayer.PlayerInput.GetMouseInput();

        Vector3 vertRotAxis = targetTransform.InverseTransformDirection(Camera.main.transform.TransformDirection(Vector3.right)).normalized;
        Vector3 horRotAxis = targetTransform.InverseTransformDirection(Camera.main.transform.TransformDirection(Vector3.up)).normalized;
        vertRotAxis = Vector3.Scale(vertRotAxis, rotationAxisVector);
        horRotAxis = Vector3.Scale(horRotAxis, rotationAxisVector);

        float horizontalRot = mouseInput.x * -rotationSpeed;
        float verticalRot = mouseInput.y * rotationSpeed;

        targetTransform.Rotate(vertRotAxis, verticalRot);
        targetTransform.Rotate(horRotAxis, horizontalRot);
    }


    internal Quaternion CalculateNearestSnapAngle()
    {
        Vector3 start = startingRotation * Vector3.forward;

        float currentAxisRotation = Vector3.SignedAngle(start, targetTransform.rotation * Vector3.forward, startingRotation * rotationAxisVector);
        float targetAxisRotation = CalculateNearestSnapAngle(currentAxisRotation);

        float snapAngleDelta = Mathf.DeltaAngle(currentAxisRotation, targetAxisRotation);

        return  startingRotation * Quaternion.Euler(rotationAxisVector * targetAxisRotation);
    }

    internal float CalculateNearestSnapAngle(float value)
    {
        if (axisSnappingPositions < 1)
            return value;

        float rawValue = value;

        value = Mathf.Repeat(value, 360);
        int snapDelta = 360 / (int)axisSnappingPositions;

        return Mathf.Round(value / snapDelta) * snapDelta;
    }

    IEnumerator SmoothToRotation(Quaternion targetRotation)
    {
        Quaternion startingRotion = targetTransform.rotation;

        float elapsedTime = 0;

        while (elapsedTime < SMOOTH_SNAP_TIME)
        {
            elapsedTime += Time.deltaTime;
            float lerpT = elapsedTime / SMOOTH_SNAP_TIME;
            targetTransform.rotation = Quaternion.Lerp(startingRotion, targetRotation, lerpT);
            yield return null;
        }

        targetTransform.rotation = targetRotation;
        activeSnapCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (targetTransform != null)
        {
            if (rotationAxis == RotationAxis.X)
                Debug.DrawLine(targetTransform.position + targetTransform.right * .2f, targetTransform.position - targetTransform.right * .2f, Color.yellow);
            else if (rotationAxis == RotationAxis.Y)
                Debug.DrawLine(targetTransform.position + targetTransform.up * .2f, targetTransform.position - targetTransform.up * .2f, Color.yellow);
            else
                Debug.DrawLine(targetTransform.position + targetTransform.forward * .2f, targetTransform.position - targetTransform.forward * .2f, Color.yellow);
        }
    }
}