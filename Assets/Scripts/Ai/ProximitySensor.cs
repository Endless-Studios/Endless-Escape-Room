using System;
using Sight;
using UnityEngine;

namespace Ai
{
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
                
                if(distance > maxProximityDistance)
                    continue;

                Ray ray = new Ray(transformPosition, toVector.normalized);
                
                if (Physics.Raycast(ray, distance, proximityBlockingMask))
                    continue;

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