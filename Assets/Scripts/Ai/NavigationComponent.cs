using System;
using System.Collections;
using System.Runtime.InteropServices;
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

        public Vector3 Destination { get; private set; }

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
            facade.OnWalkedThroughDoorway -= HandleOnWalkedThroughDoorway;
        }

        private bool isTraversingLink;
        private Vector3 deltaPosition;
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