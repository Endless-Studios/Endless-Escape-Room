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

    private void Awake()
    {
        if (targetTransform == null)
            targetTransform = transform;

        startingRotation = targetTransform.rotation; //should snap happen on awake?
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
        Vector3 rotationAxisVector;

        if (rotationAxis == RotationAxis.X) rotationAxisVector = Vector3.right;
        else if (rotationAxis == RotationAxis.Y) rotationAxisVector = Vector3.up;
        else rotationAxisVector = Vector3.forward;

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
        Vector3 rotationAxisVector;

        if (rotationAxis == RotationAxis.X) rotationAxisVector = Vector3.right;
        else if (rotationAxis == RotationAxis.Y) rotationAxisVector = Vector3.up;
        else rotationAxisVector = Vector3.forward;

        Vector3 start = startingRotation * Vector3.forward;

        float currentAxisRotation = Vector3.SignedAngle(start, targetTransform.rotation * Vector3.forward, startingRotation * rotationAxisVector);
        float targetAxisRotation = CalculateNearestSnapAngle(currentAxisRotation);

        float snapAngleDelta = Mathf.DeltaAngle(currentAxisRotation, targetAxisRotation);

        Debug.Log("SAD: " + snapAngleDelta);

        return targetTransform.rotation * Quaternion.Euler(rotationAxisVector * snapAngleDelta);
    }

    internal float CalculateNearestSnapAngle(float value)
    {
        if (axisSnappingPositions < 1)
            return value;

        float rawValue = value;

        value = Mathf.Repeat(value, 360);

        int snapDelta = 360 / (int)axisSnappingPositions;
        float result = Mathf.Round(value / snapDelta) * snapDelta;
        Debug.Log($" Raw {rawValue} | Result {result}");
        return result;
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

    private void OnDrawGizmos()
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