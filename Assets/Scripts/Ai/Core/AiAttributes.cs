using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class defines values that are used to control Ai behavior that are not changed during runtime and need to
    /// be accessed by multiple classes or visual scripting nodes. This component should be placed on the root object of
    /// the Ai prefab. 
    /// </summary>
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
        [SerializeField] private float attackDamage;

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
        public float AttackDamage => attackDamage;
    }
}