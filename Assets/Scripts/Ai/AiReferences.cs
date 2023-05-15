using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
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