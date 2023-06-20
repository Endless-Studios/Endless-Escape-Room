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
        public static readonly List<SpawnPoint> ActiveSpawnPoints = new List<SpawnPoint>();
        
        private void OnEnable()
        {
            ActiveSpawnPoints.Add(this);   
        }

        private void OnDisable()
        {
            ActiveSpawnPoints.Remove(this);
        }
        
    }
}