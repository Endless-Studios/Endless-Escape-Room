using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class should be attached to the Ai and referenced by the AiFacade. This class tracks the time since the Ai  
    /// last "wandered far" and also contains the threshold for when an ai should "wander far".
    /// </summary>
    internal class WanderFarComponent : MonoBehaviour
    {
        [SerializeField] private float wanderFarThreshold;
        
        public float WanderFarThreshold => wanderFarThreshold;
        public float TimeSinceLastWander { get; private set; }

        /// <summary>
        /// Increments TimeSinceLastWander by the deltaTime parameter. Delta time should be the amount of time that has
        /// passed since this update was called last. Likely Time.deltaTime if called during Update or Time.fixedDeltaTime
        /// if called during FixedUpdate.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateLastPatrolTime(float deltaTime)
        {
            TimeSinceLastWander += deltaTime;
        }

        /// <summary>
        /// Sets TimeSinceLastWander to zero.
        /// </summary>
        public void ResetLastWanderFarTime()
        {
            TimeSinceLastWander = 0;
        }
    }
}