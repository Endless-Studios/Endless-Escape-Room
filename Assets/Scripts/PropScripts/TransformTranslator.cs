using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for modifying the transforms local position, local rotation and local scale.
/// </summary>
public class TransformTranslator : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float positionMultiplier = 1;
    [SerializeField] private float rotationMultiplier = 1;
    [SerializeField] private float scaleMultiplier = 1;

    private void OnValidate()
    {
        if(targetTransform == null)
            targetTransform = transform;
    }

    public void TranslatePosition(Vector3 positionDelta)
    {
        targetTransform.localPosition += positionDelta * positionMultiplier;
    }

    public void TranslatePositionX(float xDelta)
    {
        TranslatePosition(new Vector3(xDelta, 0, 0));
    }

    public void TranslatePositionY(float yDelta)
    {
        TranslatePosition(new Vector3(0, yDelta, 0));
    }

    public void TranslatePositionZ(float zDelta)
    {
        TranslatePosition(new Vector3(0, 0, zDelta));
    }

    public void TranslateRotation(Vector3 rotationDelta)
    {
        targetTransform.localEulerAngles += rotationDelta * rotationMultiplier;
    }

    public void TranslateRotationX(float xDelta)
    {
        TranslateRotation(new Vector3(xDelta, 0, 0));
    }

    public void TranslateRotationY(float yDelta)
    {
        TranslateRotation(new Vector3(0, yDelta, 0));
    }

    public void TranslateRotationZ(float zDelta)
    {
        TranslateRotation(new Vector3(0, 0, zDelta));
    }

    public void TranslateScale(Vector3 scaleDelta)
    {
        targetTransform.localScale += scaleDelta * scaleMultiplier;
    }

    public void TranslateScaleX(float xDelta)
    {
        TranslateScale(new Vector3(xDelta, 0, 0));
    }

    public void TranslateScaleY(float yDelta)
    {
        TranslateScale(new Vector3(0, yDelta, 0));
    }

    public void TranslateScaleZ(float zDelta)
    {
        TranslateScale(new Vector3(0, 0, zDelta));
    }

}
