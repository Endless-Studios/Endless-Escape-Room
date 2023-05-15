using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    public class NavigationComponent : AiComponent
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator animator;
        [SerializeField] private float navigationTolerance;

        public float NavigationTolerance => navigationTolerance;
        
        public bool IsMoving { get; private set; }
        public bool HasDestination { get; private set; }
        public Vector3 Destination { get; set; }
        
        private bool isTraversingLink;
        private Vector3 deltaPosition;

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
            Entity.OnWalkedThroughDoorway += HandleOnWalkedThroughDoorway;
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

            Entity.WalkingThroughDoorway();
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
            Entity.OnWalkedThroughDoorway -= HandleOnWalkedThroughDoorway;
        }
    }
}