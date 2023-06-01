using System.ComponentModel;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    /// <summary>
    /// Used to manage navigation behavior by providing project wide navigation values and provides utility functions for
    /// usage with the NavMesh.
    /// </summary>
    public class Navigation : MonoBehaviourSingleton<Navigation>
    {
        [SerializeField] private NavMeshSurface[] surfaces;
        [SerializeField] private bool buildNavigationOnGameStart;
        [SerializeField] private float linkDisableTime;
        [SerializeField] private float meshSampleTolerance;
        [SerializeField] private int maxNavigationSamples;
        [SerializeField] private bool shouldDebugNavigation;

        public float LinkDisableTime => linkDisableTime;
        public float MeshSampleTolerance => meshSampleTolerance;
        public int MaxNavigationSamples => maxNavigationSamples;
        private static bool ShouldDebugNavigation => Instance && Instance.shouldDebugNavigation;

        private void OnValidate()
        {
            if (surfaces == null || surfaces.Length == 0)
            {
                surfaces = GetComponents<NavMeshSurface>();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            
            if (!buildNavigationOnGameStart) 
                return;
            
            foreach (NavMeshSurface surface in surfaces)
            {
                surface.BuildNavMesh();
            }
        }

        /// <summary>
        /// Calculates the total path distance of a navMeshPath
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        public static float GetPathDistance(NavMeshPath path)
        {
            switch (path.status)
            {
                case NavMeshPathStatus.PathInvalid:
                    Debug.LogWarning("Invalid path, returning float.MaxValue");
                    return float.MaxValue;
                case NavMeshPathStatus.PathPartial:
                    if(ShouldDebugNavigation)
                        Debug.LogWarning("Returning value for partial path distance");
                    goto case NavMeshPathStatus.PathComplete;
                case NavMeshPathStatus.PathComplete:
                    float totalDistance = 0f;
                    for (int i = 0; i < path.corners.Length - 1; i++)
                    {
                        Vector3 segmentStartPoint = path.corners[i];
                        Vector3 segmentEndPoint = path.corners[i + 1];
                        totalDistance += Vector3.Distance(segmentStartPoint, segmentEndPoint);
                    }
                    return totalDistance;
                default:
                    throw new InvalidEnumArgumentException();
            }
        }
    }
}