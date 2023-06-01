using System.Collections.Generic;
using UnityEngine;
// ReSharper disable LocalVariableHidesMember

namespace Ai
{
    /// <summary>
    /// This class manages all the renderers the Ai uses. This class should be attached to the root GameObject and
    /// referenced by the AiReferences component;
    /// </summary>
    public class VisualComponent : MonoBehaviour
    {
        [SerializeField] private List<Renderer> renderers;

        private void OnValidate()
        {
            if (renderers == null || renderers.Count == 0)
            {
                renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
            }
        }
        
        /// <summary>
        /// Enables all renderers referenced by the renderers list.
        /// </summary>
        public void EnableRenderers()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
        }

        /// <summary>
        /// Disables all renderers referenced by the renderers list.
        /// </summary>
        public void DisableRenderers()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }
}