using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// Simple object to define a spawn point for the Ai. It adds/removes itself to/from the ActiveSpawnPoints list based
    /// on its enabled state. 
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        private void OnEnable()
        {
            SpawnLocationManager.Instance.ActiveSpawnPoints.Add(this);   
        }

        private void OnDisable()
        {
            SpawnLocationManager.Instance.ActiveSpawnPoints.Remove(this);
        }
        
    }
}