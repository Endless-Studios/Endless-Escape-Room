using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class tracks the time since the Ai fidgeted and also contains the threshold for when an ai should fidget.
    /// This component should be attached to the Ai prefab root game object.
    /// </summary>
    public class FidgetComponent : MonoBehaviour
    {
        [SerializeField] private float fidgetThreshold;
        
        public float FidgetThreshold => fidgetThreshold;
        public float TimeSinceLastFidget { get; private set; }
        public bool IsFidgeting { get; private set; }

        /// <summary>
        /// Increments TimeSinceLastFidget by the deltaTime parameter. Delta time should be the amount of time that has
        /// passed since this update was called last. Likely Time.deltaTime if called during Update or Time.fixedDeltaTime
        /// if called during FixedUpdate.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateLastFidgetTime(float deltaTime)
        {
            TimeSinceLastFidget += deltaTime;
        }

        /// <summary>
        /// Sets TimeSinceLastFidget to zero.
        /// </summary>
        public void ResetLastFidgetTime()
        {
            TimeSinceLastFidget = 0f;
        }

        /// <summary>
        /// Sets IsFidgeting to false.
        /// </summary>
        public void StopFidgeting()
        {
            IsFidgeting = false;
        }
    }
}