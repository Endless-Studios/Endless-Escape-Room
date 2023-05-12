using System.Collections.Generic;
using UnityEngine;
// ReSharper disable LocalVariableHidesMember

namespace Ai
{
    /// <summary>
    /// This class should be attached to the root GameObject and referenced by the Facade. This class manages all the
    /// Colliders the Ai uses.
    /// </summary>
    internal class CollisionComponent : MonoBehaviour
    {
        [SerializeField] private List<Collider> colliders;
        
        /// <summary>
        /// Disables all the colliders referenced by the colliders list.
        /// </summary>
        public void DisableCollision()
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }    
        }

        /// <summary>
        /// Enables all the colliders referenced by the colliders list.
        /// </summary>
        public void EnableCollision()
        {
            foreach (Collider collider in colliders)
            {
                collider.enabled = true;
            }
        }
    }
}