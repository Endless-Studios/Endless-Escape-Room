using UnityEngine;

namespace Ai
{
    internal class NavigationComponent : MonoBehaviour
    {
        [field: SerializeField] public float NavigationTolerance { get; private set; }
        public bool HasDestination { get; private set; }
        
        public Vector3 Destination { get; private set; }

    }
}