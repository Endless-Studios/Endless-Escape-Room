using System;
using System.Collections.Generic;
using Sight;
using Unity.Mathematics;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This component is analogous to the eyes. It should be attached to the head of the Ai model and aligned with its
    /// forward vector.
    /// </summary>
    public class SightSensor : MonoBehaviour, ISense
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

        public event Action<Stimulus> OnSensedStimulus;

        private readonly Dictionary<SightTarget, float> awarenessThisFrame = new Dictionary<SightTarget, float>();

        /// <summary>
        /// Checks los against each SightTarget in the scene.
        /// </summary>
        public void CheckLos()
        {
            awarenessThisFrame.Clear();
            foreach (SightTarget sightTarget in SightTarget.SightTargets)
            {
                float totalAwarenessThisFrame = 0;
                foreach (LosProbe losProbe in sightTarget.LosProbes)
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
                        {
                            totalAwarenessThisFrame += losProbeBaseValue * viewAngleScalar * distanceScalar;
                        }
                    }
                }

                if (totalAwarenessThisFrame > 0)
                {
                    awarenessThisFrame.Add(sightTarget, totalAwarenessThisFrame);
                }
            }

            foreach (KeyValuePair<SightTarget, float> awarenessPair in awarenessThisFrame)
            {
                Stimulus stimulus = new Stimulus
                (
                    awarenessPair.Key.transform.position,
                    Time.time,
                    math.clamp(awarenessPair.Value, 0 ,100),
                    SenseKind.Sight,
                    awarenessPair.Key
                );
                
                OnSensedStimulus?.Invoke(stimulus);
            }

        }
    }

    public interface ISense
    {
        public event Action<Stimulus> OnSensedStimulus;
    }
}