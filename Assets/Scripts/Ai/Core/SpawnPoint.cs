using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
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