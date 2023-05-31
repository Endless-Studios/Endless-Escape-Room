using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    /// <summary>
    /// Handles locating and filtering points of interest for the Ai to interact with while unaware
    /// of a player or stimulus. 
    /// </summary>
    public class FiddleComponent : AiComponent
    {
        [SerializeField] private float maxInteractionDistance;
        [SerializeField] private float reInteractTime;

        private readonly List<Interaction> recentInteractions = new List<Interaction>();

        /// <summary>
        /// This method evaluates all the points of interest and selects one that is within range and hasn't
        /// been interacted with recently.
        /// </summary>
        /// <returns>Returns a valid Point of Interest or null if none are found</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public PointOfInterest GetNextFiddleTarget()
        {
            //Collect all points of interest 
            List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>(PointOfInterest.PointsOfInterest);
            
            //Remove those we have recently interacted with
            foreach (Interaction interaction in recentInteractions)
            {
                pointsOfInterest.Remove(interaction.PointOfInterest);
            }

            //Associate a pointOfInterest with a float representing the distance it will take to reach it
            List<PointOfInterestDistancePair> potentialFidgetTarget = new List<PointOfInterestDistancePair>();

            foreach (PointOfInterest pointOfInterest in pointsOfInterest)
            {
                //Calculate the path to the point of interest
                NavMeshPath path = new NavMeshPath();
                if(!NavMesh.SamplePosition(pointOfInterest.InteractionPoint, out NavMeshHit hit, attributes.NavSampleDistance, NavMesh.AllAreas))
                {
                    continue;
                }
                NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path);
                
                //If we can reach the point of interest, record the distance to it
                switch (path.status)
                {
                    case NavMeshPathStatus.PathComplete:
                        float distance = Navigation.GetPathDistance(path);
                        if(distance <= maxInteractionDistance)
                            potentialFidgetTarget.Add(new PointOfInterestDistancePair(pointOfInterest, distance));
                        break;
                    case NavMeshPathStatus.PathPartial:
                        float offset = Vector3.Distance(path.corners[path.corners.Length - 1], hit.position);
                        if (offset < .1f)
                        {
                            distance = Navigation.GetPathDistance(path);
                            if(distance <= maxInteractionDistance)
                                potentialFidgetTarget.Add(new PointOfInterestDistancePair(pointOfInterest, distance));
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

            //If there are no valid points of interest return null
            if (potentialFidgetTarget.Count == 0)
                return null;
            
            //Sort all the valid points of interest by the distance to them
            potentialFidgetTarget.Sort();
            
            //Return the closest point of interest
            return potentialFidgetTarget[0].PointOfInterest;
        }
        

        /// <summary>
        /// Adds the given point of interest to the list of recent interactions.
        /// </summary>
        /// <param name="pointOfInterest"></param>
        public void InteractWithPointOfInterest(PointOfInterest pointOfInterest)
        {
            recentInteractions.Add(new Interaction(pointOfInterest, Time.time));
        }

        private void Update()
        {
            CheckReInteractTimes(Time.time);
        }

        private void CheckReInteractTimes(float currentTime)
        {
            for (int i = recentInteractions.Count - 1; i >= 0; i--)
            {
                Interaction interaction = recentInteractions[i];
                if (currentTime > interaction.TimeOfInteraction + reInteractTime)
                    recentInteractions.RemoveAt(i);
            }
        }
    }
}