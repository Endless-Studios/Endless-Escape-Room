using System;
using Sight;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This component checks for Sense Targets nearby and invokes its OnSensedStimulus event whenever
    /// a target is within range and not blocked by some object.
    /// </summary>
    public class ProximitySensor : MonoBehaviour, ISense
    {
        [SerializeField] private LayerMask proximityBlockingMask;
        [SerializeField] private float maxProximityDistance;
        [SerializeField] private float baseProximityValue;
        
        public event Action<Stimulus> OnSensedStimulus;

        public void CheckProximitySense()
        {
            foreach (SenseTarget senseTarget in SenseTarget.SenseTargets)
            {
                Vector3 transformPosition = transform.position;
                Vector3 toVector = senseTarget.transform.position - transformPosition;
                float distance = toVector.magnitude;
                
                //Check that the sense target is within our maximum range. Break if they are not.
                if(distance > maxProximityDistance)
                    continue;

                Ray ray = new Ray(transformPosition, toVector.normalized);
                
                //Check if there is an object between us and the sense target. Break if there is
                if (Physics.Raycast(ray, distance, proximityBlockingMask))
                    continue;

                //Calculate the value of the stimulus based on the distance to the target and invoke
                //the OnSensedStimulus event.
                float awareness = Mathf.Lerp(baseProximityValue, 0, distance / maxProximityDistance);
                
                Stimulus stimulus = new Stimulus
                (
                    senseTarget.transform.position,
                    Time.time,
                    awareness,
                    SenseKind.Proximity
                );
                
                OnSensedStimulus?.Invoke(stimulus);
            }
        }
    }
}