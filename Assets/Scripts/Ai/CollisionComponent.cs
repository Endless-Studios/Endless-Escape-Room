using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    internal class CollisionComponent : MonoBehaviour
    {
        [SerializeField] private List<Collider> colliders;
        
        public void DisableCollision()
        {
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }    
        }

        public void EnableCollision()
        {
            foreach (Collider col in colliders)
            {
                col.enabled = true;
            }
        }
    }
}