using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    public class Navigation : MonoBehaviourSingleton<Navigation>
    {
        [SerializeField] private NavMeshSurface surface;
        
        protected override void Awake()
        {
            base.Awake();
            BuildNavigation();
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
                var obstacle = thresholdObject.AddComponent<NavMeshObstacle>();
                obstacle.carving = true;
                obstacle.center = new Vector3(0, obstacle.center.y, 0);
            }
            
            surface.BuildNavMesh();

            List<GameObject> floorList = new(floorObjects);

            foreach (GameObject floorObject in floorObjects)
            {
                floorObject.AddComponent<Floor>();
            }

            while (floorList.Count > 0)
            {
                GameObject roomSeed = floorList[0];
                floorList.RemoveAt(0);
                var room = roomSeed.AddComponent<Room>();
                List<GameObject> reachableFloors = new(); 
                
                foreach (GameObject floorObject in floorList)
                {
                    if(floorObject == roomSeed)
                        continue;

                    NavMeshPath path = new ();
                    NavMesh.CalculatePath(roomSeed.transform.position, floorObject.transform.position, NavMesh.AllAreas, path);
                    
                    if(path.status == NavMeshPathStatus.PathComplete)
                        reachableFloors.Add(floorObject);
                }
                
                reachableFloors.Add(roomSeed);
                room.InitializeFloorObjects(reachableFloors);
                
                foreach (GameObject floorObject in reachableFloors)
                {
                    floorList.Remove(floorObject);
                }
            }

            List<NavMeshLink> links = new();

            foreach (GameObject thresholdObject in thresholdObjects)
            {
                var obstacle = thresholdObject.GetComponent<NavMeshObstacle>();
                var link = thresholdObject.AddComponent<NavMeshLink>();
                links.Add(link);
                Vector3 obstacleSize = obstacle.size;
                link.startPoint = new Vector3(0, 0, obstacleSize.z + .5f);
                link.endPoint = new Vector3(0, 0, -obstacleSize.z - .5f);
            }

            var overlappedColliders = new Collider[5];

            foreach (GameObject floorObject in floorObjects)
            {
                foreach (Transform tform in floorObject.GetComponentsInChildren<Transform>())
                {
                    tform.gameObject.layer = LayerMask.NameToLayer("Floor");
                }
            }

            foreach (NavMeshLink navMeshLink in links)
            {
                int numCol = Physics.OverlapSphereNonAlloc(navMeshLink.gameObject.transform.position + navMeshLink.startPoint, .1f, overlappedColliders, LayerMask.GetMask("Floor"));
                
                if (numCol == 0)
                {
                    Debug.LogError("Malformed link", navMeshLink);
                    continue;
                }
                
                Room room1;
                Room room2;

                var floorObject = overlappedColliders[0].GetComponentInParent<Floor>();
                
                if (Room.FloorMap.TryGetValue(floorObject.gameObject, out Room room))
                {
                    room1 = room;
                }
                else
                {
                    Debug.LogError("Malformed link", navMeshLink);
                    continue;
                }
                
                numCol = Physics.OverlapSphereNonAlloc(navMeshLink.gameObject.transform.position + navMeshLink.endPoint, .1f, overlappedColliders, LayerMask.GetMask("Floor"));
                
                if (numCol == 0)
                {
                    Debug.LogError("Malformed link", navMeshLink);
                    continue;
                }
                
                floorObject = overlappedColliders[0].GetComponentInParent<Floor>();

                if (Room.FloorMap.TryGetValue(floorObject.gameObject, out room))
                {
                    room2 = room;
                }
                else
                {
                    Debug.LogError("Malformed link", navMeshLink);
                    continue;
                }
                
                if(room1 == room2)
                    continue;

                if(!room1.ConnectedRooms.Contains(room2))
                    room1.ConnectedRooms.Add(room2);
                if(!room2.ConnectedRooms.Contains(room1))
                    room2.ConnectedRooms.Add(room1);
            }

            foreach (GameObject floorObject in floorObjects)
            {
                foreach (Transform tform in floorObject.GetComponentsInChildren<Transform>())
                {
                    tform.gameObject.layer = LayerMask.NameToLayer("Default");
                }
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