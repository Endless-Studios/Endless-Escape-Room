using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ai
{
    public class Floor : MonoBehaviour
    {
        public readonly static Dictionary<Collider, Floor> FloorObjectByColliderKey = new();

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