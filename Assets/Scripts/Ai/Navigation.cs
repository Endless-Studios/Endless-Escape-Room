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
                NavMeshObstacle obstacle = thresholdObject.AddComponent<NavMeshObstacle>();
                obstacle.carving = true;
                obstacle.center = new Vector3(0, obstacle.center.y, 0);
            }
            
            surface.BuildNavMesh();

            List<GameObject> floorList = new List<GameObject>(floorObjects);

            foreach (GameObject floorObject in floorObjects)
            {
                Floor floor = floorObject.AddComponent<Floor>();
                Collider[] childColliders = floorObject.GetComponentsInChildren<Collider>();
                floor.InitializeFloorObject(childColliders);
                if (NavMesh.SamplePosition(floorObject.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    bool foundPosition = false;
                    foreach (Collider childCollider in childColliders)
                    {
                        if (Vector3.Distance(childCollider.ClosestPoint(hit.position), hit.position) < .2f)
                        {
                            floor.NavigationSamplePosition = hit.position;
                            foundPosition = true;
                            break;
                        }
                    }

                    if (!foundPosition)
                        floor.NavigationSamplePosition = floorObject.transform.position;
                }
                else
                {
                    floor.NavigationSamplePosition = floorObject.transform.position;
                }
            }

            while (floorList.Count > 0)
            {
                GameObject roomSeed = floorList[0];
                floorList.RemoveAt(0);
                Room room = roomSeed.AddComponent<Room>();
                List<GameObject> reachableFloors = new List<GameObject>();

                Vector3 roomSamplePosition = roomSeed.GetComponent<Floor>().NavigationSamplePosition;
                
                foreach (GameObject floorObject in floorList)
                {
                    Vector3 floorSamplePosition = floorObject.GetComponent<Floor>().NavigationSamplePosition;

                    NavMeshPath path = new NavMeshPath();
                    NavMesh.CalculatePath(roomSamplePosition, floorSamplePosition, NavMesh.AllAreas, path);
                    
                    if(path.status == NavMeshPathStatus.PathComplete)
                        reachableFloors.Add(floorObject);
                }
                
                reachableFloors.Add(roomSeed);
                room.InitializeFloorObjects(reachableFloors);
                
                foreach (GameObject floorObject in reachableFloors)
                {
                    floorList.Remove(floorObject);
                    floorObject.GetComponent<Floor>().Room = room;
                }
            }

            List<NavMeshLink> links = new List<NavMeshLink>();

            foreach (GameObject thresholdObject in thresholdObjects)
            {
                NavMeshObstacle obstacle = thresholdObject.GetComponent<NavMeshObstacle>();
                NavMeshLink link = thresholdObject.AddComponent<NavMeshLink>();
                links.Add(link);
                Vector3 obstacleSize = obstacle.size;
                link.startPoint = new Vector3(0, 0, obstacleSize.z + .5f);
                link.endPoint = new Vector3(0, 0, -obstacleSize.z - .5f);
            }

            Collider[] overlappedColliders = new Collider[5];

            foreach (GameObject floorObject in floorObjects)
            {
                foreach (Transform floorTransform in floorObject.GetComponentsInChildren<Transform>())
                {
                    floorTransform.gameObject.layer = LayerMask.NameToLayer("Floor");
                }
            }
            
            foreach (NavMeshLink navMeshLink in links)
            {
                int numberOfColliders = Physics.OverlapSphereNonAlloc(navMeshLink.transform.position, .5f, overlappedColliders, LayerMask.GetMask("Floor"));
                List<Room> hitRooms = new List<Room>();
                for (int i = 0; i < numberOfColliders; i++)
                {
                    Collider overlappedCollider = overlappedColliders[i];
                    if (Floor.FloorObjectByColliderKey.TryGetValue(overlappedCollider, out Floor floor) && Room.FloorMap.TryGetValue(floor.gameObject, out Room room))
                    {
                        if(!hitRooms.Contains(room))
                            hitRooms.Add(room);
                    }
                }

                switch (hitRooms.Count)
                {
                    case 0 or 1:
                        Debug.LogWarning("No room connection", navMeshLink);
                        break;
                    case 2:
                        if(!hitRooms[0].ConnectedRooms.Contains(hitRooms[1]))
                            hitRooms[0].ConnectedRooms.Add(hitRooms[1]);
                        if(!hitRooms[1].ConnectedRooms.Contains(hitRooms[0]))
                            hitRooms[1].ConnectedRooms.Add(hitRooms[0]);
                        break;
                    case > 2:
                        Debug.Log("Hit too many rooms", navMeshLink);
                        break;
                }
            }

            foreach (GameObject floorObject in floorObjects)
            {
                foreach (Transform floorTransform in floorObject.GetComponentsInChildren<Transform>())
                {
                    floorTransform.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
        }

        private void ReparentObjects(IEnumerable<GameObject> collection)
        {
            foreach (GameObject collectionElement in collection)
            {
                collectionElement.transform.parent = transform;
            }
        }
    }
}