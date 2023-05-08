using UnityEngine;

namespace Ai
{
    internal class PatrolComponent : MonoBehaviour
    {
        [field: SerializeField] public float PatrolThreshold { get; private set; }
        
        public float TimeSinceLastPatrol { get; private set; }

        public void UpdateLastPatrolTime(float deltaTime) => TimeSinceLastPatrol += deltaTime;

        public void ResetLastPatrolTime() => TimeSinceLastPatrol = 0;

        public void StartPatrolling()
        {
            
        }
    }
}