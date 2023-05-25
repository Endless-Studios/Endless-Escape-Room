using UnityEngine;

namespace Ai
{
    public class AiAttributes : MonoBehaviour
    {
        [SerializeField] private float wanderNearDistance;
        [SerializeField] private float wanderFarDistance;

        public float WanderNearDistance => wanderNearDistance;
        public float WanderFarDistance => wanderFarDistance;
    }
}