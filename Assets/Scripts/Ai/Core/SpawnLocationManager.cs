using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// Consolidates spawn locations for the Ai. 
    /// </summary>
    public class SpawnLocationManager : MonoBehaviourSingleton<SpawnLocationManager>
    {
        [SerializeField] private LayerMask lineOfSightMask;

        /// <summary>
        /// This function looks at all active spawn points and filters out spawn points that have line of sight to the
        /// player. Will output a random spawn point if all spawn points have line of sight to the player. Will return false
        /// if there are no active spawn points.
        /// </summary>
        /// <param name="point">The position of the spawn point</param>
        /// <returns>Returns true if a valid location was found</returns>
        public bool TryGetSpawnLocation(out Vector3 point)
        {
            point = Vector3.zero;
            
            //Return false if there are no active spawn points
            if (SpawnPoint.ActiveSpawnPoints.Count == 0)
                return false;
            
            Vector3 playerPos = PlayerCore.LocalPlayer.transform.position;
            List<Vector3> points = new List<Vector3>();

            //Raycast against the player from all spawn points
            foreach (SpawnPoint spawnPoint in SpawnPoint.ActiveSpawnPoints)
            {
                Vector3 spawnPosition = spawnPoint.transform.position;
                Vector3 losSamplePosition = spawnPosition + Vector3.up * 2;
                Vector3 toVector = playerPos - losSamplePosition;
                Ray ray = new Ray(losSamplePosition, toVector.normalized);

                //If we didn't hit an object then we can see the player and should skip this spawn point
                if (!Physics.Raycast(ray, toVector.magnitude, lineOfSightMask))
                    continue;
                
                points.Add(spawnPosition);
            }
            
            //If we have spawn points without line of sight to the player we set our output point to one of those positions randomly
            if (points.Count > 0)
            {
                int index = Random.Range(0, points.Count);
                point = points[index];
            }
            //If all of the spawn points have line of sight to the player set our output to one of those
            else
            {
                int index = Random.Range(0, SpawnPoint.ActiveSpawnPoints.Count);
                point = SpawnPoint.ActiveSpawnPoints[index].transform.position;
            }
            
            //Return true since we are always going to return a valid point if we got here
            return true;
        }
    }
}