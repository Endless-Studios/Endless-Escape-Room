using Sight;
using UnityEngine;

namespace Ai
{
    public class SightSensor : MonoBehaviour
    {
        [SerializeField] private LayerMask sightBlockingMask;
        [SerializeField] private float maxViewConeHalfAngle;
        
        public void CheckLos()
        {
            foreach (SightTarget sightTarget in SightTarget.SightTargets)
            {
                foreach (LosProbe losProbe in sightTarget.LosProbes)
                {
                    Vector3 transformPosition = transform.position;
                    Vector3 toVector = losProbe.transform.position - transformPosition;
                    
                    if(Vector3.Angle(toVector, transform.forward) > maxViewConeHalfAngle)
                        continue;
                    
                    Ray ray = new(transformPosition, toVector.normalized);
                    if (Physics.Raycast(ray, out RaycastHit hit, toVector.magnitude, sightBlockingMask))
                    {
                        if (hit.collider == losProbe.LosCollider)
                        {
                            //Debug.Log($"I can see your {losProbe.ProbeKind}");
                        }
                    }
                }
            }
        }
    }
}