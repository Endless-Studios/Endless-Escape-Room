using System;
using System.Collections.Generic;
using Sight;
using UnityEngine;
using UnityEngine.Events;

namespace Ai
{
    public class AwarenessComponent : MonoBehaviour
    {
        [SerializeField] private float noticeThreshold;
        [SerializeField] private float visibilityThreshold;
        private readonly Dictionary<SenseTarget, float> visibilityBySightTarget = new Dictionary<SenseTarget, float>();
        private List<ISense> senses;

        public Stimulus CurrentStimulus { get; private set; }
        public SenseTarget Target { get; private set; }

        public UnityEvent GainedNewStimulus;
        public UnityEvent OnGainedTarget;
        public UnityEvent OnLostTarget;

        private void Awake()
        {
            senses = new List<ISense>(GetComponentsInChildren<ISense>());
            foreach (ISense sense in senses)
            {
                sense.OnSensedStimulus += HandleOnSensedStimulus;
            }
        }

        private void HandleOnSensedStimulus(Stimulus stimulus)
        {
            switch (stimulus.SenseKind)
            {
                case SenseKind.Sight:
                    if (visibilityBySightTarget.ContainsKey(stimulus.SenseTarget))
                    {
                        visibilityBySightTarget[stimulus.SenseTarget] += stimulus.Value;
                    }
                    else
                    {
                        visibilityBySightTarget.Add(stimulus.SenseTarget, stimulus.Value);
                    }
                    break;
                case SenseKind.Hearing:
                case SenseKind.Proximity:
                case SenseKind.Undefined:
                    if (CurrentStimulus is null || stimulus.Value > CurrentStimulus.Value)
                    {
                        CurrentStimulus = stimulus;
                        GainedNewStimulus?.Invoke();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ProcessStimuli()
        {
            if (Target)
            {
                if (visibilityBySightTarget.ContainsKey(Target))
                {
                    visibilityBySightTarget.Clear();
                    return;
                }
                
                Stimulus lastKnowLocation = new Stimulus(Target.transform.position, Time.time, 100, SenseKind.Undefined);
                CurrentStimulus = lastKnowLocation;
                Target = null;
                GainedNewStimulus?.Invoke();
                OnLostTarget?.Invoke();
            }
            else
            {
                (SenseTarget sightTarget, float value) mostVisibleTarget = (null, float.MinValue);
                foreach (KeyValuePair<SenseTarget,float> sightPair in visibilityBySightTarget)
                {
                    if (sightPair.Value > mostVisibleTarget.value)
                    {
                        mostVisibleTarget = (sightPair.Key, sightPair.Value);
                    }
                }

                if (!mostVisibleTarget.sightTarget)
                {
                    visibilityBySightTarget.Clear();
                    return;
                }
                
                if (mostVisibleTarget.value > visibilityThreshold)
                {
                    Target = mostVisibleTarget.sightTarget;
                    LostInterest();
                    OnGainedTarget?.Invoke();
                }
                else if (mostVisibleTarget.value > noticeThreshold)
                {
                    Stimulus noticeLocation = new Stimulus(mostVisibleTarget.sightTarget.transform.position, Time.time, mostVisibleTarget.value, SenseKind.Undefined);
                    HandleOnSensedStimulus(noticeLocation);
                }
            }
            visibilityBySightTarget.Clear();
        }

        public void LostInterest()
        {
            CurrentStimulus = null;
        }
    }
}