using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class manages all the Colliders the Ai uses. This class should be attached to the root GameObject and
    /// referenced by the AiReferences component.
    /// </summary>
    public class CollisionComponent : MonoBehaviour
    {
        [SerializeField] private Collider[] colliders;

        private void OnValidate()
        {
            if (colliders.Length == 0)
            {
                colliders = GetComponentsInChildren<Collider>();
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