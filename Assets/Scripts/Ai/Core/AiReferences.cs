using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    /// <summary>
    ///  Holds references to components that need to be referenced by other components or are deeper in the hierarchy
    /// and need to be exposed to the Ai state graph. Should be placed on the root object of the Ai prefab.
    /// </summary>
    public class AiReferences : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private NavigationComponent navigationComponent;
        [SerializeField] private SightSensor sightSensor;
        [SerializeField] private HearingSensor hearingSensor;
        [SerializeField] private ProximitySensor proximitySensor;
        [SerializeField] private AwarenessComponent awarenessComponent;
        [SerializeField] private AnimationComponent animationComponent;
        [SerializeField] private ThresholdBehavior boredThreshold;
        [SerializeField] private ThresholdBehavior wanderThreshold;
        [SerializeField] private ThresholdBehavior wanderFarThreshold;
        [SerializeField] private ThresholdBehavior fidgetThreshold;
        [SerializeField] private ThresholdBehavior fiddleThreshold;
        [SerializeField] private ThresholdBehavior searchThreshold;
        [SerializeField] private ThresholdBehavior investigateThreshold;


        public Animator Animator => animator;
        public NavMeshAgent Agent => agent;
        public NavigationComponent NavigationComponent => navigationComponent;
        public SightSensor SightSensor => sightSensor;
        public HearingSensor HearingSensor => hearingSensor;
        public ProximitySensor ProximitySensor => proximitySensor;
        public AwarenessComponent AwarenessComponent => awarenessComponent;
        public AnimationComponent AnimationComponent => animationComponent;
        public ThresholdBehavior BoredThreshold => boredThreshold;
        public ThresholdBehavior WanderThreshold => wanderThreshold;
        public ThresholdBehavior WanderFarThreshold => wanderFarThreshold;
        public ThresholdBehavior FidgetThreshold => fidgetThreshold;
        public ThresholdBehavior FiddleThreshold => fiddleThreshold;
        public ThresholdBehavior SearchThreshold => searchThreshold;
        public ThresholdBehavior InvestigateThreshold => investigateThreshold;
    }
}