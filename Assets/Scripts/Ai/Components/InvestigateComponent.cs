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
        [SerializeField] private float maxInvestigateDistance;
        [SerializeField] private float reInvestigateTime;

        public UnityEvent OnFinishedInvestigating = new UnityEvent();

        private readonly List<Interaction> recentInvestigations = new List<Interaction>();

        /// <summary>
        /// This method evaluates all the hideouts and selects one within range that hasn't been
        /// searched recently. 
        /// </summary>
        /// <returns>Returns a valid hideout that isn't the hideout the player is in unless all other
        /// hideouts have been searched or null if the player isn't in a hideout. </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Hideout GetNextInvestigateTarget()
        {
            //Collect all hideouts
            List<Hideout> hideouts = new List<Hideout>(Hideout.Hideouts);

            //Filter out those that have been recently searched
            foreach (Interaction interaction in recentInvestigations)
            {
                if (interaction.PointOfInterest is Hideout hideout)
                {
                    hideouts.Remove(hideout);
                }
            }

            //If the player is in a hideout remove that one from consideration for now
            if(gameplayInfo.PlayersHideout)
                hideouts.Remove(gameplayInfo.PlayersHideout);

            //Associate a hideout with a float representing the distance it will take to reach it
            List<PointOfInterestDistancePair> potentialInvestigationTarget = new List<PointOfInterestDistancePair>();

            foreach (Hideout hideout in hideouts)
            {
                //Calculate the path to the hideout
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(transform.position, hideout.InteractionPoint, NavMesh.AllAreas, path);

                //If we can reach the hideout, record the distance to it
                switch (path.status)
                {
                    case NavMeshPathStatus.PathComplete:
                        float distance = Navigation.GetPathDistance(path);
                        if(distance <= maxInvestigateDistance)
                            potentialInvestigationTarget.Add(new PointOfInterestDistancePair(hideout, distance));
                        break;
                    case NavMeshPathStatus.PathPartial:
                        break;
                    case NavMeshPathStatus.PathInvalid:
                        Debug.LogWarning("Invalid path, check source and target positions to ensure validity");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //If there are no valid hideouts return the players current hideout(it is okay if this is null)
            if (potentialInvestigationTarget.Count == 0)
                return gameplayInfo.PlayersHideout;
            
            //Sort all the valid hideouts by the distance to them
            potentialInvestigationTarget.Sort();

            //Return the closest hideout
            return (Hideout)potentialInvestigationTarget[0].PointOfInterest;
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
        
    }
}