using System.Collections.Generic;
using UnityEngine;
// ReSharper disable LocalVariableHidesMember

namespace Ai
{
    public class VisualComponent : MonoBehaviour
    {
        [SerializeField] private List<Renderer> renderers;

        public void EnableRenderers()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = true;
            }
        }

        public void DisableRenderers()
        {
            foreach (Renderer renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }
}