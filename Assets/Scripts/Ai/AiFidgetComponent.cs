using UnityEngine;

namespace Ai
{
    internal class AiFidgetComponent : MonoBehaviour
    {
        [field: SerializeField] public float FidgetThreshold { get; private set; }
        public float TimeSinceLastFidget { get; private set; }
        
        public bool IsFidgeting { get; private set; }

        public void UpdateLastFidgetTime(float deltaTime) => TimeSinceLastFidget += deltaTime;

        public void ResetLastFidgetTime() => TimeSinceLastFidget = 0f;

        public void StopFidgeting()
        {
            IsFidgeting = false;
        }

        public void StartFidgeting()
        {
            
        }
    }
}