using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Rotatable : Grabbable
{
    public enum RotationAxis { X, Y, Z }

    [HideInInspector] public UnityEvent<float> OnFinishedRotation = new UnityEvent<float>(); //rotation finished event sends resulting rotation delta from starting rotation (0 - 360)
    [HideInInspector] public  UnityEvent<int> OnFinishedRotationSnap = new UnityEvent<int>(); //rotation finished event sends resulting snap position id

    protected override string DefaultInteractionText => "Rotate";

    [SerializeField] private Transform targetTransform;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private RotationAxis rotationAxis;

    [Header("Snapping")]
    [SerializeField, Min(0)] private int axisSnappingPositions = 0;
    [SerializeField, Min(.05f)] private float smoothSnapTime = .1f;

    private Coroutine activeSnapCoroutine;
    private Quaternion startingRotation; //all rotations and snapping are relative to starting rotation

    private Vector3 RotationAxisVector //rotation axis for mathematical calculations
    {
        get
        {
            if (rotationAxis == RotationAxis.X)
                return Vector3.right;
            else if (rotationAxis == RotationAxis.Y)
                return Vector3.up;
            else
                return Vector3.forward;
        }
    }

    private Vector3 RotationForwardAxisVector
    {
        get
        {
            if (rotationAxis == RotationAxis.X)
                return Vector3.forward;
            else if (rotationAxis == RotationAxis.Y)
                return Vector3.left;
            else
                return Vector3.up;
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
        float currentAxisRotation = Vector3.SignedAngle(startingRotation * RotationForwardAxisVector, targetTransform.localRotation * RotationForwardAxisVector, startingRotation * RotationAxisVector);
        currentAxisRotation = Mathf.Repeat(currentAxisRotation, 360); //0 - 360
        OnFinishedRotation.Invoke(currentAxisRotation);

        if (axisSnappingPositions > 0)
        {
            int snapDistance = 360 / (int)axisSnappingPositions;
            int snapPositionID = Mathf.RoundToInt(currentAxisRotation / snapDistance); //the id where the snap is landing            
            snapPositionID = (int)Mathf.Repeat(snapPositionID, axisSnappingPositions); // 0 - positionCount
            float targetAxisRotation = snapPositionID * snapDistance; //the desired rotation to snap to
            Quaternion targetRotation = startingRotation * Quaternion.Euler(RotationAxisVector * targetAxisRotation); //rotate on the target axis from starting rotation            
            activeSnapCoroutine = StartCoroutine(SmoothToRotation(targetRotation, snapPositionID));
        }
    }

    public override void HandleUpdate()
    {
        Vector2 mouseInput = PlayerCore.LocalPlayer.PlayerInput.GetMouseInput();

        //Use Inverse Transfrom Direction to rotate axis so when we use mouse input to rotate it will  behave based on camera's realtive position to the object
        Vector3 vertrticalRotataionAxis = targetTransform.InverseTransformDirection(Camera.main.transform.TransformDirection(Vector3.right)).normalized;
        Vector3 horizontalRotationAxis = targetTransform.InverseTransformDirection(Camera.main.transform.TransformDirection(Vector3.up)).normalized;

        //zero out vectors that we arent rotating on
        vertrticalRotataionAxis = Vector3.Scale(vertrticalRotataionAxis, RotationAxisVector);
        horizontalRotationAxis = Vector3.Scale(horizontalRotationAxis, RotationAxisVector);

        targetTransform.Rotate(vertrticalRotataionAxis, mouseInput.y * rotationSpeed);
        targetTransform.Rotate(horizontalRotationAxis, mouseInput.x * -rotationSpeed);
    }

    IEnumerator SmoothToRotation(Quaternion targetRotation, int snapPositionID)
    {
        Quaternion smoothStartingRotion = targetTransform.localRotation;
        float elapsedTime = 0;

        while (elapsedTime < smoothSnapTime)
        {
            elapsedTime += Time.deltaTime;
            float interpolationPoint = elapsedTime / smoothSnapTime;
            targetTransform.localRotation = Quaternion.Lerp(smoothStartingRotion, targetRotation, interpolationPoint);
            yield return null;
        }

        targetTransform.localRotation = targetRotation;
        OnFinishedRotationSnap.Invoke(snapPositionID);

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