using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Ai
{
    internal class VisualComponent : MonoBehaviour
    {
        [SerializeField] private List<Renderer> renderers;

        internal void EnableRenderers()
        {
            foreach (Renderer ren in renderers)
            {
                ren.enabled = true;
            }
        }

        internal void DisableRenderers()
        {
            foreach (Renderer ren in renderers)
            {
                ren.enabled = false;
            }
        }
    }
}