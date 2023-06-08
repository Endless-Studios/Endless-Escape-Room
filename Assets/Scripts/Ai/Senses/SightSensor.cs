using System;
using Sight;
using Unity.Mathematics;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This component is analogous to the eyes. It should be attached to the head of the Ai model and aligned with its
    /// forward vector.
    /// </summary>
    public class SightSensor : Sense
    {
        [SerializeField] private LayerMask sightBlockingMask;
        [SerializeField] private float centralVisionScalar;
        [SerializeField] private float nearPeripheralThreshold;
        [SerializeField] private float nearPeripheralScalar;
        [SerializeField] private float farPeripheralThreshold;
        [SerializeField] private float farPeripheralScalar;
        [SerializeField] private float maxViewConeHalfAngle;
        [SerializeField] private float maxViewDistance;
        [SerializeField] private float closeViewThreshold;
        [SerializeField] private float closeViewDistanceScalar;
        [SerializeField] private float losProbeBaseValue;

        /// <summary>
        /// Checks line of sight against each SightTarget in the scene.
        /// </summary>
        public void CheckLineOfSight()
        {
            foreach (PlayerTarget senseTarget in PlayerTarget.SenseTargets)
            {
                float totalAwarenessThisFrame = 0;
                foreach (LosProbe losProbe in senseTarget.LosProbes)
                {
                    Vector3 transformPosition = transform.position;
                    Vector3 toVector = losProbe.transform.position - transformPosition;
                    
                    //If the Los probe is outside of out view cone skip the raycast or too far away
                    float viewAngle = Vector3.Angle(toVector, transform.forward);
                    float distance = toVector.magnitude;
                    if(viewAngle > maxViewConeHalfAngle || distance > maxViewDistance)
                        continue;
                    
                    float viewAngleScalar;
                    if (viewAngle < nearPeripheralThreshold)
                        viewAngleScalar = centralVisionScalar;
                    else if (viewAngle < farPeripheralThreshold)
                        viewAngleScalar = nearPeripheralScalar;
                    else
                        viewAngleScalar = farPeripheralScalar;

                    float distanceScalar;
                    if (distance > closeViewThreshold)
                    {
                        float interpolant = (distance - closeViewThreshold) / (maxViewDistance - closeViewThreshold);
                        distanceScalar = math.lerp(closeViewDistanceScalar, 0f, interpolant);
                    }
                    else
                        distanceScalar = closeViewDistanceScalar;

                    Ray ray = new Ray(transformPosition, toVector.normalized);
                    
                    if (Physics.Raycast(ray, out RaycastHit hit, distance, sightBlockingMask))
                    {
                        if (hit.collider == losProbe.LosCollider)
                            totalAwarenessThisFrame += losProbeBaseValue * viewAngleScalar * distanceScalar;
                    }
                }

                if (totalAwarenessThisFrame <= 0)
                    continue;

                Stimulus stimulus = new Stimulus
                (
                    senseTarget.transform.position,
                    Time.time,
                    math.clamp(totalAwarenessThisFrame, 0 ,100),
                    SenseKind.Sight,
                    senseTarget
                );
                
                SensedStimulus(stimulus);
            }
        }
    }
}