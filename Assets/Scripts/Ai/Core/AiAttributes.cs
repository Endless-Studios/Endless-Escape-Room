using UnityEngine;

namespace Ai
{
    public class AiAttributes : MonoBehaviour
    {
        [SerializeField] private float minWanderNearDistance;
        [SerializeField] private float wanderNearDistance;
        [SerializeField] private float minWanderFarDistance;
        [SerializeField] private float wanderFarDistance;
        [SerializeField] private float navSampleDistance;
        [SerializeField] private float navigationTolerance;
        [SerializeField] private float maxAttackDistance;
        [SerializeField] private float approachDistance;
        [SerializeField] private float maxAngleDifference;
        [SerializeField] private float maxAngularSpeed;
        [SerializeField] private float targetMovementExtrapolationTime;

        public float MinWanderNearDistance => minWanderNearDistance;
        public float WanderNearDistance => wanderNearDistance;
        public float MinWanderFarDistance => minWanderFarDistance;
        public float WanderFarDistance => wanderFarDistance;
        public float NavSampleDistance => navSampleDistance;
        public float NavigationTolerance => navigationTolerance;
        public float MaxAttackDistance => maxAttackDistance;
        public float ApproachDistance => approachDistance;
        public float MaxAngleDifference => maxAngleDifference;
        public float MaxAngularSpeed => maxAngularSpeed;
        public float TargetMovementExtrapolationTime => targetMovementExtrapolationTime;
    }
}