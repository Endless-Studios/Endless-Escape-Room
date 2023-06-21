using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// Consolidates spawn locations for the Ai. 
    /// </summary>
    public class SpawnLocationManager : MonoBehaviourSingleton<SpawnLocationManager>
    {
        public  readonly List<SpawnPoint> ActiveSpawnPoints = new List<SpawnPoint>();
        [SerializeField] private LayerMask lineOfSightMask;

        /// <summary>
        /// This function looks at all active spawn points and filters out spawn points that have line of sight to the
        /// player. Will output a random spawn point if all spawn points have line of sight to the player. Will return false
        /// if there are no active spawn points.
        /// </summary>
        /// <param name="point">The position of the spawn point</param>
        /// <param name="rotation">The rotation of the spawn point</param>
        /// <returns>Returns true if a valid location was found</returns>
        public bool TryGetSpawnLocation(out Vector3 point, out Quaternion rotation)
        {
            point = Vector3.zero;
            rotation = Quaternion.identity;
            
            //Return false if there are no active spawn points
            if (ActiveSpawnPoints.Count == 0)
                return false;
            
            Vector3 playerPos = PlayerCore.LocalPlayer.transform.position;
            List<Transform> points = new List<Transform>();

            //Raycast against the player from all spawn points
            foreach (SpawnPoint spawnPoint in ActiveSpawnPoints)
            {
                Vector3 spawnPosition = spawnPoint.transform.position;
                Vector3 losSamplePosition = spawnPosition + Vector3.up * 2;
                Vector3 toVector = playerPos - losSamplePosition;
                Ray ray = new Ray(losSamplePosition, toVector.normalized);

                //If we didn't hit an object then we can see the player and should skip this spawn point
                if (!Physics.Raycast(ray, toVector.magnitude, lineOfSightMask))
                    continue;
                
                points.Add(spawnPoint.transform);
            }
            
            //If we have spawn points without line of sight to the player we set our output point to one of those positions randomly
            if (points.Count > 0)
            {
                int index = Random.Range(0, points.Count);
                point = points[index].position;
                rotation = points[index].rotation;
            }
            //If all of the spawn points have line of sight to the player set our output to one of those
            else
            {
                int index = Random.Range(0, ActiveSpawnPoints.Count);
                point = ActiveSpawnPoints[index].transform.position;
            }
            
            //Return true since we are always going to return a valid point if we got here
            return true;
        }
    }
}