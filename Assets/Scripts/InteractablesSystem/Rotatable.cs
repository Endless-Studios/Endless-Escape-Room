using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Rotatable : Grabbable
{
    public enum RotationAxis { X, Y, Z }

    [SerializeField] UnityEvent<float> OnFinishedRotation = new UnityEvent<float>(); //rotation finished event sends resulting rotation delta from starting rotation (0 - 360)
    [SerializeField] UnityEvent<int> OnFinishedRotationSnap = new UnityEvent<int>(); //rotation finished event sends resulting snap index

    protected override string DefaultInteractionText => "Rotate";

    [SerializeField] private Transform targetTransform;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private RotationAxis rotationAxis;

    [Header("Snapping")]
    [SerializeField, Min(0)] private int axisSnappingPositions = 0;
    [SerializeField, Min(.05f)] private float smoothSnapTime = .1f;

    private Coroutine activeSnapCoroutine;
    private Quaternion startingRotation; //all rotations and snapping are relative to starting rotation

    private Vector3 rotationAxisVector //rotation axis for mathematical calculations
    {
        get
        {
            if (rotationAxis == RotationAxis.X) return Vector3.right;
            else if (rotationAxis == RotationAxis.Y) return Vector3.up;
            else return Vector3.forward;
        }
    }

    private Vector3 rotationForwardAxisVector
    {
        get
        {
            if (rotationAxis == RotationAxis.X) return Vector3.forward;
            else if (rotationAxis == RotationAxis.Y) return Vector3.left;
            else return Vector3.up;
        }
    }

    private void Awake()
    {
        if (targetTransform == null)
            targetTransform = transform;

        startingRotation = targetTransform.localRotation;
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
        //the angle between starting forward direction and current forward direction
        float currentAxisRotation = Vector3.SignedAngle(startingRotation * rotationForwardAxisVector, targetTransform.localRotation * rotationForwardAxisVector, startingRotation * rotationAxisVector);
        currentAxisRotation = Mathf.Repeat(currentAxisRotation, 360); //0 - 360
        OnFinishedRotation.Invoke(currentAxisRotation);

        if (axisSnappingPositions > 0)
        {
            int snapDelta = 360 / (int)axisSnappingPositions;
            int snapIndex = Mathf.RoundToInt(currentAxisRotation / snapDelta); //the index where the snap is landing            
            snapIndex = (int)Mathf.Repeat(snapIndex, axisSnappingPositions); // 0 - positionCount
            float targetAxisRotation = snapIndex * snapDelta; //the desired rotation to snap to
            Quaternion targetRotation = startingRotation * Quaternion.Euler(rotationAxisVector * targetAxisRotation); //rotate on the target axis from starting rotation            
            activeSnapCoroutine = StartCoroutine(SmoothToRotation(targetRotation, snapIndex));
        }
    }

    public override void HandleUpdate()
    {
        Vector2 mouseInput = PlayerCore.LocalPlayer.PlayerInput.GetMouseInput();

        //Use Inverse Transfrom Direction to rotate axis so when we use mouse input to rotate it will  behave based on camera's realtive position to the object
        Vector3 vertRotAxis = targetTransform.InverseTransformDirection(Camera.main.transform.TransformDirection(Vector3.right)).normalized;
        Vector3 horRotAxis = targetTransform.InverseTransformDirection(Camera.main.transform.TransformDirection(Vector3.up)).normalized;

        //zero out vectors that we arent rotating on
        vertRotAxis = Vector3.Scale(vertRotAxis, rotationAxisVector);
        horRotAxis = Vector3.Scale(horRotAxis, rotationAxisVector);

        targetTransform.Rotate(vertRotAxis, mouseInput.y * rotationSpeed);
        targetTransform.Rotate(horRotAxis, mouseInput.x * -rotationSpeed);
    }

    IEnumerator SmoothToRotation(Quaternion targetRotation, int snapIndex)
    {
        Quaternion smoothStartingRotion = targetTransform.localRotation;
        float elapsedTime = 0;

        while (elapsedTime < smoothSnapTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpT = elapsedTime / smoothSnapTime;
            targetTransform.localRotation = Quaternion.Lerp(smoothStartingRotion, targetRotation, lerpT);
            yield return null;
        }

        targetTransform.localRotation = targetRotation;
        OnFinishedRotationSnap.Invoke(snapIndex);

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