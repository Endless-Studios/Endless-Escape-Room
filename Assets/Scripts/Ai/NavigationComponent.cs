using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    internal class NavigationComponent : AiComponent
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private float navigationTolerance;
        [SerializeField] private float overlapRadius;
        [SerializeField] private LayerMask overlapMask;

        public float NavigationTolerance => navigationTolerance;
        public bool IsMoving { get; private set; }
        public bool HasDestination { get; private set; }
        public Room LastRoom { get; private set; }
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
        private bool isTraversingLink;
        private Vector3 deltaPosition;
        private readonly Collider[] results = new Collider[5];

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
            Transform agentTransform = agent.transform;
            agentTransform.position = agentTransform.TransformPoint(animator.transform.localPosition);
            animator.transform.position = Vector3.zero;
            agent.CompleteOffMeshLink();
            CurrentRoom = GetCurrentRoom();
            facade.OnWalkedThroughDoorway -= HandleOnWalkedThroughDoorway;
        }

        private Room GetCurrentRoom()
        {
            int overlappedColliders = Physics.OverlapSphereNonAlloc(transform.position, overlapRadius, results, overlapMask);
            
            if (overlappedColliders == 0)
            {
                Debug.Log("Overlapped no colliders, this shouldn't be possible", this);
                return null;
            }

            for (int i = 0; i < overlappedColliders; i++)
            {
                Collider overlappedCollider = results[i];

                if (!Floor.FloorObjectByColliderKey.TryGetValue(overlappedCollider, out Floor floor))
                {
                    continue;
                }

                if (Room.FloorMap.TryGetValue(floor.gameObject, out Room room))
                    return room;
            }
            
            Debug.Log("Overlapped no floor objects, this shouldn't be possible", this);
            return null;
        }
    }
}