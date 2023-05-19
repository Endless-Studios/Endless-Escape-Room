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
        [SerializeField] private float stimuliDeclineRate;
        private readonly Dictionary<SightTarget, float> visibilityBySightTarget = new Dictionary<SightTarget, float>();
        private SightTarget target;
        private List<ISense> senses;

        public Stimulus CurrentStimulus { get; private set; }

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
                    if (visibilityBySightTarget.ContainsKey(stimulus.SightTarget))
                    {
                        visibilityBySightTarget[stimulus.SightTarget] += stimulus.Value;
                    }
                    else
                    {
                        visibilityBySightTarget.Add(stimulus.SightTarget, stimulus.Value);
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
            if (target)
            {
                if (visibilityBySightTarget.ContainsKey(target)) 
                    return;
                
                Stimulus lastKnowLocation = new Stimulus(target.transform.position, Time.time, 100, SenseKind.Undefined);
                OnLostTarget?.Invoke();
                CurrentStimulus = lastKnowLocation;
                target = null;
                GainedNewStimulus?.Invoke();
            }
            else
            {
                (SightTarget sightTarget, float value) mostVisibleTarget = (null, float.MinValue);
                foreach (KeyValuePair<SightTarget,float> sightPair in visibilityBySightTarget)
                {
                    if (sightPair.Value > mostVisibleTarget.value)
                    {
                        mostVisibleTarget = (sightPair.Key, sightPair.Value);
                    }
                }

                if (!mostVisibleTarget.sightTarget) 
                    return;
                
                if (mostVisibleTarget.value > visibilityThreshold)
                {
                    target = mostVisibleTarget.sightTarget;
                    OnGainedTarget?.Invoke();
                }
                else if (mostVisibleTarget.value > noticeThreshold)
                {
                    Stimulus noticeLocation = new Stimulus(mostVisibleTarget.sightTarget.transform.position, Time.time, mostVisibleTarget.value, SenseKind.Undefined);
                    HandleOnSensedStimulus(noticeLocation);
                }
            }
        }

        public void DecreaseStimuliValue(float deltaTime)
        {
            if (CurrentStimulus is not null)
            {
                CurrentStimulus.Value -= stimuliDeclineRate * deltaTime;
                if (CurrentStimulus.Value <= 0)
                    CurrentStimulus = null;
            }
            visibilityBySightTarget.Clear();
        }

        public void LostInterest()
        {
            CurrentStimulus = null;
        }
    }
}