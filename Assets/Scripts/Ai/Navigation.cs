using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    public class Navigation : MonoBehaviourSingleton<Navigation>
    {
        [SerializeField] private NavMeshSurface surface;
        
        public void Start()
        {
            BuildNavigation();
            surface.BuildNavMesh();
        }

        private void BuildNavigation()
        {
            GameObject[] floorObjects = GameObject.FindGameObjectsWithTag("Floor");
            GameObject[] thresholdObjects = GameObject.FindGameObjectsWithTag("Threshold");
            GameObject[] stairObjects = GameObject.FindGameObjectsWithTag("Stair");
            GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
            
            ReparentObjects(floorObjects);
            ReparentObjects(thresholdObjects);
            ReparentObjects(stairObjects);
            ReparentObjects(walls);

            foreach (GameObject thresholdObject in thresholdObjects)
            {
                var link = thresholdObject.AddComponent<NavMeshLink>();
                var obstacle = thresholdObject.AddComponent<NavMeshObstacle>();
                obstacle.carving = true;
                obstacle.center = new Vector3(0, obstacle.center.y, 0);
                Vector3 obstacleSize = obstacle.size;
                link.startPoint = new Vector3(0, 0, obstacleSize.z + .5f);
                link.endPoint = new Vector3(0, 0, -obstacleSize.z - .5f);
            }
        }

        private void ReparentObjects(IEnumerable<GameObject> collection)
        {
            foreach (GameObject obj in collection)
            {
                obj.transform.parent = transform;
            }
        }
    }
}