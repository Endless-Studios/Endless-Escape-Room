using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Ai
{
    /// <summary>
    /// Handles the locating and filtering of player hideouts when the Ai has recently seen the player
    /// and is trying to locate them.
    /// </summary>
    public class InvestigateComponent : AiComponent
    {
        [SerializeField] private LayerMask interactionPointMask; 
        [SerializeField] private float investigateActionDelay;
        [SerializeField] private float maxInvestigateDistance;
        [SerializeField] private float reInvestigateTime;
        [HideInInspector] public bool ShouldWander;
        [HideInInspector] public bool ShouldInteract;
        [HideInInspector] public bool ShouldForceInteraction;
 
        public UnityEvent OnFinishedInvestigating = new UnityEvent();
        public float InvestigateActionDelay => investigateActionDelay;

        private readonly List<Interaction> recentInvestigations = new List<Interaction>();

        public PointOfInterest GetNextInvestigateTarget()
        {
            if (ShouldForceInteraction)
            {
                if (gameplayInfo.CurrentStimulus is SoundStimulus { PointOfInterest: not null } soundStimulus)
                    return soundStimulus.PointOfInterest;
            }

            List<PointOfInterest> pointOfInterests;

            if (gameplayInfo.AiAwarenessState == AiAwarenessState.Searching)
                pointOfInterests = new List<PointOfInterest>(PointOfInterest.PointsOfInterest);
            else
                pointOfInterests = new List<PointOfInterest>(Hideout.Hideouts);
                
            pointOfInterests.Remove(gameplayInfo.PlayersHideout);
            
            //Filter out those that have been recently searched
            foreach (Interaction interaction in recentInvestigations)
            {
                if (interaction.PointOfInterest is Hideout hideout)
                {
                    pointOfInterests.Remove(hideout);
                }
            }
            
            //Associate a hideout with a float representing the distance it will take to reach it
            List<PointOfInterestDistancePair> potentialInvestigationTarget = new List<PointOfInterestDistancePair>();

            foreach (PointOfInterest pointOfInterest in pointOfInterests)
            {
                //Calculate the path to the hideout
                NavMeshPath path = new NavMeshPath();
                if(!NavMesh.SamplePosition(pointOfInterest.InteractionPoint, out NavMeshHit hit, attributes.NavSampleDistance, NavMesh.AllAreas))
                {
                    continue;
                }
                NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path);

                //If we can reach the hideout, record the distance to it
                switch (path.status)
                {
                    case NavMeshPathStatus.PathComplete:
                        float distance = Navigation.GetPathDistance(path);
                        if(distance <= maxInvestigateDistance)
                            potentialInvestigationTarget.Add(new PointOfInterestDistancePair(pointOfInterest, distance));
                        break;
                    case NavMeshPathStatus.PathPartial:
                        float offset = Vector3.Distance(path.corners[path.corners.Length - 1], hit.position);
                        if (offset < .1f)
                        {
                            distance = Navigation.GetPathDistance(path);
                            if(distance <= maxInvestigateDistance)
                                potentialInvestigationTarget.Add(new PointOfInterestDistancePair(pointOfInterest, distance));
                            break;
                        }
                        Debug.LogWarning("Partial path, check the position of the interact point on this point of interest", pointOfInterest.gameObject);
                        break;
                    case NavMeshPathStatus.PathInvalid:
                        Debug.LogWarning("Invalid path, check source and target positions to ensure validity");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            if (potentialInvestigationTarget.Count == 0)
                return null;
            
            //Sort all the valid hideouts by the distance to them
            potentialInvestigationTarget.Sort();
            
            recentInvestigations.Add(new Interaction(potentialInvestigationTarget[0].PointOfInterest, Time.time));

            //Return the closest hideout
            return potentialInvestigationTarget[0].PointOfInterest;
        }

        /// <summary>
        /// Wrapper method for invoking the OnFinishedInvestigating event from Visual Scripting.
        /// </summary>
        public void FinishedInvestigating()
        {
            OnFinishedInvestigating.Invoke();
        }

        /// <summary>
        /// Adds the provided hideout to the list of recently investigated hideouts.
        /// </summary>
        /// <param name="hideout"></param>
        public void SearchHideout(Hideout hideout)
        {
            recentInvestigations.Add(new Interaction(hideout, Time.time));
        }

        private void Update()
        {
            CheckReInvestigateTimes(Time.time);
        }

        private void CheckReInvestigateTimes(float currentTime)
        {
            for (int i = recentInvestigations.Count - 1; i >= 0; i--)
            {
                Interaction interaction = recentInvestigations[i];
                if(currentTime > interaction.TimeOfInteraction + reInvestigateTime)
                    recentInvestigations.RemoveAt(i);
            }
        }

        public bool IsSoundStimulus(Stimulus stimulus)
        {
            return stimulus is SoundStimulus;
        }

        public bool CanSeeSoundStimulusSource(SoundStimulus stimulus)
        {
            if (stimulus.PointOfInterest is null)
                return false;

            Collider collider = stimulus.PointOfInterest.LineOfSightCollider;

            Vector3 startingPos = references.SightSensor.transform.position;
            Vector3 targetPos = stimulus.PointOfInterest.LineOfSightCollider.bounds.center;
            Vector3 toVector = targetPos - startingPos;
            
            bool didHit = Physics.Raycast(startingPos, toVector.normalized, out RaycastHit hit, toVector.magnitude, interactionPointMask);

            return didHit && hit.collider == collider;
        }
    }
}