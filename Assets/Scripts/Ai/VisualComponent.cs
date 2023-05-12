using System.Collections.Generic;
using UnityEngine;
// ReSharper disable LocalVariableHidesMember

namespace Ai
{
    internal class VisualComponent : MonoBehaviour
    {
        [SerializeField] private List<Renderer> renderers;

        internal void EnableRenderers()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
        }

        internal void DisableRenderers()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }
}