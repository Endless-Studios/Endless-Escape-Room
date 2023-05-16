using System.Collections.Generic;
using UnityEngine;
// ReSharper disable LocalVariableHidesMember

namespace Ai
{
    /// <summary>
    /// This class manages all the Colliders the Ai uses. This class should be attached to the root GameObject and
    /// referenced by the AiReferences component.
    /// </summary>
    public class CollisionComponent : MonoBehaviour
    {
        [SerializeField] private List<Collider> colliders;

        private void OnValidate()
        {
            if (colliders.Count == 0)
            {
                colliders = new List<Collider>(GetComponentsInChildren<Collider>());
            }
        }

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