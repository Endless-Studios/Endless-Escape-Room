using System.Collections.Generic;
using Sight;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    public class TargetTracker : AiComponent
    {
        [SerializeField] private float maxSampleTime;
        [SerializeField] private float navmeshSampleDistance;

        private void Awake()
        {
            references.AwarenessComponent.OnGainedTarget.AddListener(HandleOnGainedTarget);
        }

        private int MaxSampledPositions => Mathf.RoundToInt(maxSampleTime / Time.fixedDeltaTime);
        
        private readonly List<Vector3> recentPositions = new List<Vector3>();

        public Vector3 GetEstimatedPosition(float estimationTime)
        {
            Vector3 displacement = Vector3.zero;
            
            foreach (Vector3 position in recentPositions)
            {
                displacement += position;
            }

            Vector3 velocity = displacement / (recentPositions.Count * Time.fixedDeltaTime);

            Vector3 estimatedPosition = recentPositions[recentPositions.Count - 1] + velocity * estimationTime;
            
            
            if (NavMesh.SamplePosition(estimatedPosition, out NavMeshHit hit, navmeshSampleDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }

            if(NavMesh.SamplePosition(recentPositions[recentPositions.Count - 1], out hit, navmeshSampleDistance, NavMesh.AllAreas))
            {
                return hit.position;
            }
            
            return recentPositions[^1];
        }

        private void HandleOnGainedTarget()
        {
            recentPositions.Clear();
        }

        private void FixedUpdate()
        {
            SenseTarget target = references.AwarenessComponent.Target;
            if (target)
            {
                if (recentPositions.Count == MaxSampledPositions)
                {
                    recentPositions.RemoveAt(0);
                }
                recentPositions.Add(target.transform.position);
            }
        }
    }
}