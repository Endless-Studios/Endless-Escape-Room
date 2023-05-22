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
        

        public Animator Animator => animator;
        public NavMeshAgent Agent => agent;
        public NavigationComponent NavigationComponent => navigationComponent;
        public SightSensor SightSensor => sightSensor;
        public HearingSensor HearingSensor => hearingSensor;
    }
}