using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Ai
{
    /// <summary>
    /// This component handles the navigation needs of the Ai it is attached to. This component should be placed on the
    /// root game object of the Ai prefab.
    /// </summary>
    public class NavigationComponent : AiComponent
    {
        [SerializeField] private float doorApproachRotationSpeed;
        [SerializeField] private float doorApproachAngleTolerance;
        [SerializeField] private float doorApproachMovementSpeed;
        [SerializeField] private float doorApproachDistanceTolerance;

        public bool IsMoving { get; private set; }

        private bool isTraversingLink;

        public UnityEvent OnTriedLockedThreshold;

        private void Update()
        {
            if (references.Agent.isOnOffMeshLink && !isTraversingLink)
            {
                StartCoroutine(TryTraverseLink());
            }

            IsMoving = references.Agent.hasPath && !isTraversingLink;
            
            if (references.Agent.hasPath && Vector3.Distance(references.Agent.destination, transform.position) < attributes.NavigationTolerance)
                references.Agent.ResetPath();
        }

        private IEnumerator TryTraverseLink()
        {
            isTraversingLink = true;
            OffMeshLinkData linkData = references.Agent.currentOffMeshLinkData;
            LinkController linkController = LinkController.GetLinkFromEndPoints(linkData.startPos, linkData.endPos);
            Vector3 linkDirection = linkData.endPos - linkData.startPos;
            Quaternion lookRotation = Quaternion.LookRotation(linkDirection);
            references.Agent.updatePosition = false;
            references.Agent.updateRotation = false;
            bool isInteracting = false;
            
            //While we are not aligned with the navMeshLink or not close enough too it we move and rotate towards the appropriate values 
            while (Quaternion.Angle(transform.rotation, lookRotation) > doorApproachAngleTolerance || Vector3.Distance(transform.position, linkData.startPos) > doorApproachDistanceTolerance)
            {
                float angle = Quaternion.Angle(transform.rotation, lookRotation);
                if(angle > doorApproachAngleTolerance)
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, doorApproachRotationSpeed * Time.deltaTime);
                float distance = Vector3.Distance(transform.position, linkData.startPos);
                if(distance > doorApproachDistanceTolerance)
                    transform.position = Vector3.MoveTowards(transform.position, linkData.startPos, doorApproachMovementSpeed * Time.deltaTime);
                yield return null;
            }

            if (!linkController.Openable.IsOpen)
            {
                string animationName = AnimationNames.Instance.GetInteractionAnimationName(linkController.Openable.InteractionType);
                Debug.Log("Setting animation trigger for: " + animationName);
                //references.Animator.SetTrigger(animationName);
                yield return new WaitForSeconds(1f);
                //TODO: Get animations in so I can tie into the animation events
                // isInteracting = true;
                // Entity.OnFinishedInteracting.AddListener(HandleFinishedInteracting);
                // yield return new WaitUntil(() => !isInteracting);
                // Entity.OnFinishedInteracting.RemoveListener(HandleFinishedInteracting);
            }

            //If the threshold is locked we need to break out of current navigation path and disable the link temporarily
            if (linkController.Openable.IsLocked)
            {
                linkController.DisableLink(references.Agent.agentTypeID, Navigation.Instance.LinkDisableTime);
                //This flicker is intentional since it clears the current off mesh link without needing to complete it.
                references.Agent.enabled = false;
                references.Agent.enabled = true;
                gameplayInfo.Destination = transform.position;
                references.Agent.updateRotation = true;
                references.Agent.updatePosition = true;
                isTraversingLink = false;
                OnTriedLockedThreshold.Invoke();
            }
            //Otherwise we want to listen for us finishing traversal of the threshold
            else
            {
                //TODO: Branch here to handle different animations
                entity.OnWalkedThroughDoorway += HandleOnWalkedThroughThreshold;
                entity.WalkingThroughThreshold();
            }

            void HandleFinishedInteracting()
            {
                isInteracting = false;
            }
        }

        private void HandleOnWalkedThroughThreshold()
        {
            isTraversingLink = false;
            references.Agent.updateRotation = true;
            references.Agent.updatePosition = true;
            Transform agentTransform = references.Agent.transform;
            agentTransform.position = agentTransform.TransformPoint(references.Animator.transform.localPosition);
            references.Animator.transform.position = Vector3.zero;
            references.Agent.CompleteOffMeshLink();
            entity.OnWalkedThroughDoorway -= HandleOnWalkedThroughThreshold;
        }

        /// <summary>
        /// Returns a random position on the navMesh that has a path to it that is shorter than maxWanderDistance.
        /// </summary>
        public Vector3 GetWanderDestination(float maxWanderDistance, float minWanderDistance)
        {
            Vector3 transformPosition = transform.position;
            for (int i = 0; i < Navigation.Instance.MaxNavigationSamples; i++)
            {
                Vector3 direction = Random.insideUnitSphere.normalized;
                direction *= Random.Range(minWanderDistance, maxWanderDistance);
                Vector3 samplePosition = transformPosition + direction;
                
                if (!NavMesh.SamplePosition(samplePosition, out NavMeshHit hit, Navigation.Instance.MeshSampleTolerance, NavMesh.AllAreas)) 
                    continue;
                
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(transformPosition, hit.position, NavMesh.AllAreas, path);
                    
                if(path.status is NavMeshPathStatus.PathInvalid or NavMeshPathStatus.PathPartial)
                    continue;
                    
                if (Navigation.GetPathDistance(path) > maxWanderDistance)
                    continue;

                return hit.position;
            }
            Debug.LogWarning("Hit maximum number of samples for wander destination. Returning current position");
            return transformPosition;
        }
    }
}