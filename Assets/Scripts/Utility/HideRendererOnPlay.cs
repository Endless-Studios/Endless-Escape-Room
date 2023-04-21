using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideRendererOnPlay : MonoBehaviour
{
    [SerializeField] Renderer[] renderers;

    private void OnValidate()
    {
        if(renderers == null || renderers.Length == 0)
        {
            renderers = GetComponentsInChildren<Renderer>();
        }
    }

    private void Awake()
    {
        foreach(Renderer renderer in renderers)
            renderer.enabled = false;
    }
}
