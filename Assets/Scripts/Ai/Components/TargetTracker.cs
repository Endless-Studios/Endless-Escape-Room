using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    /// <summary>
    /// Tracks the position of the current target of the Ai. 
    /// </summary>
    public class TargetTracker : AiComponent
    {
        [SerializeField] private float maxSampleTime;

        private int MaxSampledPositions => Mathf.RoundToInt(maxSampleTime / Time.fixedDeltaTime);
        
        private readonly List<Vector3> recentPositions = new List<Vector3>();
        
        /// <summary>
        /// Uses the average velocity of the target across the sampled positions to extrapolate a
        /// position the target could be at. 
        /// </summary>
        /// <param name="estimationTime">How far out to extrapolate the movement</param>
        /// <returns>The extrapolated position of the target</returns>
        public Vector3 GetEstimatedPosition(float estimationTime)
        {
            Vector3 displacement = Vector3.zero;
            
            foreach (Vector3 position in recentPositions)
            {
                displacement += position;
            }

            Vector3 velocity = displacement / (recentPositions.Count * Time.fixedDeltaTime);

            Vector3 estimatedPosition = recentPositions[recentPositions.Count - 1] + velocity * estimationTime;
            
            
            if (NavMesh.SamplePosition(estimatedPosition, out NavMeshHit hit, attributes.NavSampleDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }

            if(NavMesh.SamplePosition(recentPositions[recentPositions.Count - 1], out hit, attributes.NavSampleDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
            
            return recentPositions[^1];
        }
        
        private void Awake()
        {
            references.AwarenessComponent.OnGainedTarget.AddListener(HandleOnGainedTarget);
        }
        
        private void HandleOnGainedTarget()
        {
            recentPositions.Clear();
        }

        private void FixedUpdate()
        {
            if (gameplayInfo.Target)
            {
                if (recentPositions.Count == MaxSampledPositions)
                {
                    recentPositions.RemoveAt(0);
                }
                recentPositions.Add(gameplayInfo.Target.transform.position);
            }
        }
    }
}