using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Ai
{
    public class Room : MonoBehaviour
    {
        public static readonly Dictionary<GameObject, Room> FloorMap = new();
        [field: SerializeField] public List<Room> ConnectedRooms { get; private set; } = new();
        [field: SerializeField] public List<GameObject> FloorObjects { get; private set; } = new();

        public void InitializeFloorObjects(IEnumerable<GameObject> floorObjects)
        {
            FloorObjects = new List<GameObject>(floorObjects);
            
            foreach (GameObject floorObject in FloorObjects)
            {
                FloorMap.Add(floorObject, this);
            }
        }

        public void OnDestroy()
        {
            foreach (GameObject floorObject in FloorObjects)
            {
                FloorMap.Remove(floorObject);
            }
        }

        public Room GetRandomConnectedRoom(Room lastRoom = null)
        {
            if (ConnectedRooms.Count == 0)
            {
                Debug.LogWarning("No connected rooms", this);
            }

            List<Room> copyOfConnectedRooms = new(ConnectedRooms);

            if (lastRoom)
            {
                if (copyOfConnectedRooms.Count == 1)
                    return lastRoom;

                copyOfConnectedRooms.Remove(lastRoom);
            }

            int index = Random.Range(0, copyOfConnectedRooms.Count);
            return copyOfConnectedRooms[index];
        }

        public Vector3 GetRandomNavigationPointInRoom()
        {
            int index = Random.Range(0, FloorObjects.Count);
            GameObject floor = FloorObjects[index];

            Vector2 flatVector = Random.insideUnitCircle.normalized;
            Vector3 samplePoint = new Vector3(flatVector.x, 0, flatVector.y) + floor.transform.position;
            var col = floor.GetComponentInChildren<Collider>();
            samplePoint = col.ClosestPoint(samplePoint);
            if (!NavMesh.SamplePosition(samplePoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                Debug.LogWarning("Malformed nav mesh", floor);
                return samplePoint;
            }
            return hit.position;
        }
    }
}