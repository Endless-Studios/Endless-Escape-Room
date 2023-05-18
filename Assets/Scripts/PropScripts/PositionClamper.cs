using UnityEngine;

///<summary>Component for clamping a transforms local position.</summary>
public class PositionClamper : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private bool autoClamp = false;

    [Header("X")]
    [SerializeField] private bool clampX;
    [SerializeField] private float minX = -1;
    [SerializeField] private float maxX = 1;

    [Header("Y")]
    [SerializeField] private bool clampY;
    [SerializeField] private float minY = -1;
    [SerializeField] private float maxY = 1;

    [Header("Z")]
    [SerializeField] private bool clampZ;
    [SerializeField] private float minZ = -1;
    [SerializeField] private float maxZ = 1;

    private void OnValidate()
    {
        if (targetTransform == null)
            targetTransform = transform;
    }

    void Update()
    {
        if (autoClamp)
            Clamp();
    }

    public void Clamp()
    {
        Vector3 clampedLocalPosition = targetTransform.localPosition;

        if (clampX)
            clampedLocalPosition.x = Mathf.Clamp(clampedLocalPosition.x, minX, maxX);

        if (clampY)
            clampedLocalPosition.y = Mathf.Clamp(clampedLocalPosition.y, minY, maxY);

        if (clampZ)
            clampedLocalPosition.z = Mathf.Clamp(clampedLocalPosition.z, minZ, maxZ);

        targetTransform.localPosition = clampedLocalPosition;

    }
}
