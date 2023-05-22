using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    public class FiddleComponent : AiComponent
    {
        [SerializeField] private float fiddleThreshold;
        [SerializeField] private float maxInteractionDistance;
        [SerializeField] private float reInteractTime;

        private readonly List<Interaction> recentInteractions = new List<Interaction>();

        public float FiddleThreshold => fiddleThreshold;
        public float TimeSinceLastFiddle { get; private set; }

        public void UpdateLastFiddleTime(float deltaTime)
        {
            TimeSinceLastFiddle += deltaTime;
        }

        public void ResetLastFiddleTime()
        {
            TimeSinceLastFiddle = 0f;
        }

        public PointOfInterest GetNextFiddleTarget(List<PointOfInterest> pointsToIgnore)
        {
            List<PointOfInterest> pointsOfInterest = new List<PointOfInterest>(PointOfInterest.PointsOfInterest);
            
            foreach (Interaction interaction in recentInteractions)
            {
                pointsToIgnore.Add(interaction.PointOfInterest);
            }
            
            foreach (PointOfInterest pointOfInterest in pointsToIgnore)
            {
                pointsOfInterest.Remove(pointOfInterest);
            }

            List<(PointOfInterest pointOfInterest, float distance)> potentialFidgetTarget = new List<(PointOfInterest pointOfInterest, float distance)>();

            foreach (PointOfInterest pointOfInterest in pointsOfInterest)
            {
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, pointOfInterest.InteractionPoint, NavMesh.AllAreas, path);
                
                switch (path.status)
                {
                    case NavMeshPathStatus.PathComplete:
                        float distance = Navigation.GetPathDistance(path);
                        if(distance <= maxInteractionDistance)
                            potentialFidgetTarget.Add((pointOfInterest, distance));
                        break;
                    case NavMeshPathStatus.PathPartial:
                        continue;
                    case NavMeshPathStatus.PathInvalid:
                        Debug.LogWarning("Invalid path, check source and target positions to ensure validity");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (potentialFidgetTarget.Count == 0)
                return null;
            
            potentialFidgetTarget.Sort((a,b) => a.distance.CompareTo(b.distance));
            
            return potentialFidgetTarget[0].pointOfInterest;
        }

        public void InteractWithPointOfInterest(PointOfInterest pointOfInterest)
        {
            recentInteractions.Add(new Interaction(pointOfInterest, Time.time));
            Entity.StartedInteracting(pointOfInterest);
        }

        public void CheckReInteractTimes(float currentTime)
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