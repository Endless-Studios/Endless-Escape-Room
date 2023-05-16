using Sight;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This component is analogous to the eyes. It should be attached to the head of the Ai model and aligned with its
    /// forward vector.
    /// </summary>
    public class SightSensor : MonoBehaviour
    {
        [SerializeField] private LayerMask sightBlockingMask;
        [SerializeField] private float maxViewConeHalfAngle;
        
        /// <summary>
        /// Checks los against each SightTarget in the scene.
        /// </summary>
        public void CheckLos()
        {
            foreach (SightTarget sightTarget in SightTarget.SightTargets)
            {
                foreach (LosProbe losProbe in sightTarget.LosProbes)
                {
                    Vector3 transformPosition = transform.position;
                    Vector3 toVector = losProbe.transform.position - transformPosition;
                    
                    //If the Los probe is outside of out view cone skip the raycast
                    if(Vector3.Angle(toVector, transform.forward) > maxViewConeHalfAngle)
                        continue;
                    
                    Ray ray = new Ray(transformPosition, toVector.normalized);
                    if (Physics.Raycast(ray, out RaycastHit hit, toVector.magnitude, sightBlockingMask))
                    {
                        if (hit.collider == losProbe.LosCollider)
                        {
                            //If we hit the collider trigger stimulus events.   
                        }
                    }
                }
            }
        }
    }
}