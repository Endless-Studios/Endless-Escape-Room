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
            }

            while (floorList.Count > 0)
            {
                GameObject roomSeed = floorList[0];
                floorList.RemoveAt(0);
                var room = roomSeed.AddComponent<Room>();
                List<GameObject> reachableFloors = new();

                Vector3 roomSamplePosition = roomSeed.transform.position;
                
                if (NavMesh.SamplePosition(roomSamplePosition, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    roomSamplePosition = hit.position;
                }
                
                foreach (GameObject floorObject in floorList)
                {
                    Vector3 floorSamplePosition = floorObject.transform.position;
                    
                    if (NavMesh.SamplePosition(floorObject.transform.position, out NavMeshHit floorHit, 2f, NavMesh.AllAreas))
                    {
                        floorSamplePosition = floorHit.position;
                    }

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
                Room room1;
                Room room2;
                int numCol = Physics.OverlapSphereNonAlloc(navMeshLink.gameObject.transform.position + navMeshLink.startPoint, .1f, overlappedColliders, LayerMask.GetMask("Floor"));
                
                if (numCol == 0)
                {
                    Debug.LogError("Malformed link", navMeshLink);
                    continue;
                }

                if (!Floor.FloorObjectByColliderKey.TryGetValue(overlappedColliders[0], out Floor floor))
                {
                    Debug.LogError("Malformed link", navMeshLink);
                    continue;
                }
                
                if (Room.FloorMap.TryGetValue(floor.gameObject, out Room room))
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
                
                if (!Floor.FloorObjectByColliderKey.TryGetValue(overlappedColliders[0], out floor))
                {
                    Debug.LogError("Malformed link", navMeshLink);
                    continue;
                }

                if (Room.FloorMap.TryGetValue(floor.gameObject, out room))
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