using UnityEngine;

namespace Ai
{
    public class RotationComponent : AiComponent
    {
        [SerializeField] private float rotationSpeed;

        public float RotationSpeed => rotationSpeed;

        public void RotateTowardsTarget()
        {
            
        }
    }

    public struct InteractionData
    {
        
    }
}