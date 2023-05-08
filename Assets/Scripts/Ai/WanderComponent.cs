using UnityEngine;

namespace Ai
{
    internal class WanderComponent : MonoBehaviour
    {
        [field: SerializeField] public float WanderThreshold { get; private set; }
        public float TimeSinceLastWander { get; private set; }
        
        public void UpdateLastWanderTime(float deltaTime) => TimeSinceLastWander += deltaTime;

        public void ResetLastWanderTime() => TimeSinceLastWander = 0f;

        public void StartWandering()
        {
            
        }
    }
}