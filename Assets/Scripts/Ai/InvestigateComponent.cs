using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Ai
{
    public class InvestigateComponent : MonoBehaviour
    {
        [SerializeField] private float maxInvestigateDistance;
        [SerializeField] private float reInvestigateTime;
        
        [HideInInspector] public Hideout PlayersHideout;
        public UnityEvent OnFinishedInvestigating = new UnityEvent();

        private readonly List<Interaction> recentInvestigations = new List<Interaction>();

        public Hideout GetNextInvestigateTarget()
        {
            List<Hideout> hideouts = new List<Hideout>(Hideout.Hideouts);

            foreach (Interaction interaction in recentInvestigations)
            {
                if (interaction.PointOfInterest is Hideout hideout)
                {
                    hideouts.Remove(hideout);
                }
            }

            hideouts.Remove(PlayersHideout);

            List<(Hideout hideout, float distance)> potentialInvestigationTarget = new List<(Hideout hideout, float distance)>();

            foreach (Hideout hideout in hideouts)
            {
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, hideout.InteractionPoint, NavMesh.AllAreas, path);

                switch (path.status)
                {
                    case NavMeshPathStatus.PathComplete:
                        float distance = Navigation.GetPathDistance(path);
                        if(distance <= maxInvestigateDistance)
                            potentialInvestigationTarget.Add((hideout, distance));
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

            if (potentialInvestigationTarget.Count == 0)
                return PlayersHideout;
            
            potentialInvestigationTarget.Sort((a, b) => a.distance.CompareTo(b.distance));

            return potentialInvestigationTarget[0].hideout;
        }

        public void FinishedInvestigating()
        {
            OnFinishedInvestigating.Invoke();
        }

        public void SearchHideout(Hideout hideout)
        {
            recentInvestigations.Add(new Interaction(hideout, Time.time));
        }

        public void CheckReInvestigateTimes(float currentTime)
        {
            for (int i = recentInvestigations.Count - 1; i >= 0; i--)
            {
                Interaction interaction = recentInvestigations[i];
                if(currentTime > interaction.TimeOfInteraction + reInvestigateTime)
                    recentInvestigations.RemoveAt(i);
            }
        }
    }
}