using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class should not be placed on a GameObject manually. The Navigation class will place this on appropriately
    /// tagged objects at the start of any play session. The floor class is used to facilitate the navigation of the Ai
    /// by associating an instance of the floor class with any child colliders the object may have.
    /// </summary>
    [SelectionBase]
    public class Floor : MonoBehaviour
    {
        public static readonly Dictionary<Collider, Floor> FloorObjectByColliderKey = new Dictionary<Collider, Floor>();
        public Room Room;
        public Vector3 NavigationSamplePosition;

        /// <summary>
        /// Stand in for the constructor on a MonoBehaviour. This 
        /// </summary>
        /// <param name="colliders"></param>
        public void InitializeFloorObject(IEnumerable<Collider> colliders)
        {
            floorColliders = new List<Collider>(colliders);
            foreach (Collider floorCollider in floorColliders)
            {
                FloorObjectByColliderKey.Add(floorCollider, this);
            }
        }

        private void OnDestroy()
        {
            foreach (Collider floorCollider in floorColliders)
            {
                FloorObjectByColliderKey.Remove(floorCollider);
            }
        }

        private List<Collider> floorColliders;
    }
}