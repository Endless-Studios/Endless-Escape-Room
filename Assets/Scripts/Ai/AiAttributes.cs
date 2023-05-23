using UnityEngine;

namespace Ai
{
    public class AiAttributes : MonoBehaviour
    {
        [SerializeField] private float wanderNearDistance;
        [SerializeField] private float wanderFarDistance;
        [SerializeField] private AgentType agentType;

        public float WanderNearDistance => wanderNearDistance;
        public float WanderFarDistance => wanderFarDistance;
        public AgentType AgentType => agentType;
    }
}