using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    public class SpawnLocationManager : MonoBehaviourSingleton<SpawnLocationManager>
    {
        [SerializeField] private LayerMask lineOfSightMask;

        public bool TryGetSpawnLocation(out Vector3 point)
        {
            point = Vector3.zero;
            
            if (SpawnPoint.ActiveSpawnPoints.Count == 0)
                return false;
            
            Vector3 playerPos = PlayerCore.LocalPlayer.transform.position;
            List<Vector3> points = new List<Vector3>();

            foreach (SpawnPoint spawnPoint in SpawnPoint.ActiveSpawnPoints)
            {
                Vector3 spawnPosition = spawnPoint.transform.position;
                Vector3 losSamplePosition = spawnPosition + Vector3.up * 2;
                Vector3 toVector = playerPos - losSamplePosition;
                Ray ray = new Ray(losSamplePosition, toVector.normalized);

                if (Physics.Raycast(ray, toVector.magnitude, lineOfSightMask))
                    continue;

                points.Add(spawnPosition);
            }

            if (points.Count == 0)
            {
                int index = Random.Range(0, points.Count);
                point = points[index];
            }
            else
            {
                int index = Random.Range(0, SpawnPoint.ActiveSpawnPoints.Count);
                point = SpawnPoint.ActiveSpawnPoints[index].transform.position;
            }
            
            return true;
        }
    }
}