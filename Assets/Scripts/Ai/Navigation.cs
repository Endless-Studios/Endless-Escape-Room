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
                var floor = floorObject.AddComponent<Floor>();
                Collider[] colliders = floorObject.GetComponentsInChildren<Collider>();
                floor.InitializeFloorObject(colliders);
                if (NavMesh.SamplePosition(floorObject.transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    var foundPosition = false;
                    foreach (Collider col in colliders)
                    {
                        if (Vector3.Distance(col.ClosestPoint(hit.position), hit.position) < .2f)
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

                Debug.DrawLine(floor.NavigationSamplePosition, floor.NavigationSamplePosition + Vector3.up * 2, Color.cyan, 60f);
            }

            while (floorList.Count > 0)
            {
                GameObject roomSeed = floorList[0];
                floorList.RemoveAt(0);
                var room = roomSeed.AddComponent<Room>();
                List<GameObject> reachableFloors = new();

                Vector3 roomSamplePosition = roomSeed.GetComponent<Floor>().NavigationSamplePosition;
                
                foreach (GameObject floorObject in floorList)
                {
                    Vector3 floorSamplePosition = floorObject.GetComponent<Floor>().NavigationSamplePosition;

                    NavMeshPath path = new ();
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
                int numCol = Physics.OverlapSphereNonAlloc(navMeshLink.transform.position, .5f, overlappedColliders, LayerMask.GetMask("Floor"));
                List<Room> hitRooms = new();
                for (int i = 0; i < numCol; i++)
                {
                    Collider col = overlappedColliders[i];
                    if (Floor.FloorObjectByColliderKey.TryGetValue(col, out Floor floor) && Room.FloorMap.TryGetValue(floor.gameObject, out Room room))
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