using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    [SelectionBase]
    public class Floor : MonoBehaviour
    {
        public static readonly Dictionary<Collider, Floor> FloorObjectByColliderKey = new();
        public Room Room;
        public Vector3 NavigationSamplePosition;

        public void InitializeFloorObject(IEnumerable<Collider> colliders)
        {
            floorColliders = new List<Collider>(colliders);
            foreach (Collider floorCollider in floorColliders)
            {
                FloorObjectByColliderKey.Add(floorCollider, this);
            }
        }

        public void OnDestroy()
        {
            foreach (Collider col in floorColliders)
            {
                FloorObjectByColliderKey.Remove(col);
            }
        }

        private List<Collider> floorColliders;
    }
}