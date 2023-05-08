using UnityEngine;

namespace Ai
{
    internal class NavigationComponent : MonoBehaviour
    {
        public bool HasDestination { get; private set; }
        
        public Vector3 Destination { get; private set; }
        
    }
}