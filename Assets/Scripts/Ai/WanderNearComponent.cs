using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class should be attached to the Ai and referenced by the AiFacade. This class tracks the time since the Ai  
    /// last "wandered near" and also contains the threshold for when an ai should "wander near".
    /// </summary>
    public class WanderNearComponent : MonoBehaviour
    {
        [SerializeField] private float wanderThreshold;
        [SerializeField] private float maxWanderDistance;
        
        public float WanderThreshold => wanderThreshold;
        public float MaxWanderDistance => maxWanderDistance;
        public float TimeSinceLastWander { get; private set; }
        
        /// <summary>
        /// Increments TimeSinceLastWanderFar by the deltaTime parameter. Delta time should be the amount of time that has
        /// passed since this update was called last. Likely Time.deltaTime if called during Update or Time.fixedDeltaTime
        /// if called during FixedUpdate.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateLastWanderTime(float deltaTime)
        {
            TimeSinceLastWander += deltaTime;
        }

        /// <summary>
        /// Sets TimeSinceLastWander to zero
        /// </summary>
        public void ResetLastWanderTime()
        {
            TimeSinceLastWander = 0f;
        }
    }
}