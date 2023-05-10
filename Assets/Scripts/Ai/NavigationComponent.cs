using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    internal class NavigationComponent : AiComponent
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [field: SerializeField] public float NavigationTolerance { get; private set; }
        public bool IsMoving { get; private set; }
        public bool HasDestination { get; private set; }

        public Vector3 Destination { get; set; }

        public Room CurrentRoom
        {
            get => currentRoom;
            
            private set
            {
                if (value == currentRoom)
                    return;

                LastRoom = currentRoom;
                currentRoom = value;
            }
        }

        private Room currentRoom;
        
        public Room LastRoom { get; private set; }

        private void Start()
        {
            CurrentRoom = GetCurrentRoom();
        }

        private void Update()
        {
            if (agent.isOnOffMeshLink && !isTraversingLink)
            {
                StartCoroutine(TraverseLink());
            }

            IsMoving = agent.hasPath && !isTraversingLink;

            if (agent.hasPath && Vector3.Distance(agent.destination, transform.position) < NavigationTolerance)
                agent.ResetPath();
        }

        private IEnumerator TraverseLink()
        {
            facade.OnWalkedThroughDoorway += HandleOnWalkedThroughDoorway;
            isTraversingLink = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
            OffMeshLinkData linkData = agent.currentOffMeshLinkData;
            Vector3 linkDirection = linkData.endPos - linkData.startPos;
            Quaternion lookRotation = Quaternion.LookRotation(linkDirection);
            while (Vector3.Angle(transform.forward, linkDirection) > 2f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 360 * Time.deltaTime);
                yield return null;
            }

            facade.WalkingThroughDoorway();
        }

        private void HandleOnWalkedThroughDoorway()
        {
            isTraversingLink = false;
            agent.updateRotation = true;
            agent.updatePosition = true;
            agent.transform.position = agent.transform.TransformPoint(animator.transform.localPosition);
            animator.transform.position = Vector3.zero;
            agent.CompleteOffMeshLink();
            CurrentRoom = GetCurrentRoom();
            facade.OnWalkedThroughDoorway -= HandleOnWalkedThroughDoorway;
        }

        public Room GetCurrentRoom()
        {
            int size = Physics.OverlapSphereNonAlloc(transform.position, .2f, results, LayerMask.GetMask("Default"));
            if (size == 0)
            {
                Debug.Log("Overlapped no colliders, this shouldn't be possible", this);
                return null;
            }

            for (int i = 0; i < size; i++)
            {
                Collider col = results[i];
                
                if (Room.FloorMap.TryGetValue(col.gameObject, out Room room))
                    return room;
            }
            
            Debug.Log("Overlapped no floor objects, this shouldn't be possible", this);
            return null;
        }

        private bool isTraversingLink;
        private Vector3 deltaPosition;
        private readonly Collider[] results = new Collider[5];
    }

    public abstract class AiComponent : MonoBehaviour
    {
        protected virtual void Awake()
        {
            facade = GetComponentInParent<AiFacade>();
        }

        protected AiFacade facade;
    }

    public enum LinkTraversalType
    {
        Walk,
        Run,
        Jump,
        Leap
    }
}