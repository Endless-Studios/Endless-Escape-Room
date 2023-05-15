using Unity.AI.Navigation;
using UnityEngine;

namespace Ai
{
    public class Navigation : MonoBehaviour
    {
        [SerializeField] private NavMeshSurface surface;
        
        protected void Awake()
        {
            surface.BuildNavMesh();
        }
    }
}