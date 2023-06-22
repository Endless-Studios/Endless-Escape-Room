using System;
using System.Collections.Generic;
using Sight;
using UnityEngine;
using UnityEngine.Events;

namespace Ai
{
    /// <summary>
    /// This class handles filtering stimulus and line of sight data as well as invoking several
    /// awareness and targeting related events. 
    /// </summary>
    public class AwarenessComponent : AiComponent
    {
        [SerializeField] private float noticeThreshold;
        [SerializeField] private float visibilityThreshold;
        [SerializeField] private Sense[] senses;

        public UnityEvent GainedNewStimulus;
        public UnityEvent OnGainedTarget;
        public UnityEvent OnLostTarget;
        public UnityEvent TargetEnteredHideout;
        public UnityEvent TargetLeftHideout;
        
        private readonly Dictionary<PlayerTarget, float> visibilityBySightTarget = new Dictionary<PlayerTarget, float>();

        private void Awake()
        {
            foreach (Sense sense in senses)
            {
                sense.OnSensedStimulus += HandleOnSensedStimulus;
            }

            Hideout.OnEnteredHideout += HandleEnteredHideout;
            Hideout.OnLeftHideout += HandleLeftHideout;
            
        }

        private void HandleEnteredHideout(Hideout hideout)
        {
            if (gameplayInfo.Target)
            {
                gameplayInfo.TargetHideout = hideout;
                TargetEnteredHideout.Invoke();
                return;
            }

            gameplayInfo.PlayersHideout = hideout;
        }

        private void HandleLeftHideout(Hideout hideout)
        {
            if (gameplayInfo.TargetHideout)
            {
                TargetLeftHideout.Invoke();
                gameplayInfo.TargetHideout = null;
            }

            gameplayInfo.PlayersHideout = null;
        }

        private void HandleOnSensedStimulus(Stimulus stimulus)
        {
            switch (stimulus.SenseKind)
            {
                case SenseKind.Sight:
                    SightStimulus sightStimulus = (SightStimulus)stimulus;
                    if (visibilityBySightTarget.ContainsKey(sightStimulus.PlayerTarget))
                    {
                        visibilityBySightTarget[sightStimulus.PlayerTarget] += stimulus.Value;
                    }
                    else
                    {
                        visibilityBySightTarget.Add(sightStimulus.PlayerTarget, stimulus.Value);
                    }
                    break;
                case SenseKind.Hearing:
                case SenseKind.Proximity:
                case SenseKind.Undefined:
                case SenseKind.ForceAlert:
                    if (gameplayInfo.CurrentStimulus is null || stimulus.Value > gameplayInfo.CurrentStimulus.Value)
                    {
                        gameplayInfo.CurrentStimulus = stimulus;
                        GainedNewStimulus?.Invoke();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Primary driver of the awareness system. Is called by the Fsm visual scripting graph. 
        /// </summary>
        public void ProcessStimuli()
        {
            //If we currently have a target
            if (gameplayInfo.Target)
            {
                //If we have line of sight this frame we don't care about anything else and can return
                if (visibilityBySightTarget.ContainsKey(gameplayInfo.Target))
                {
                    visibilityBySightTarget.Clear();
                    return;
                }
                
                //If we lost line of sight this frame provide a stimulus at the targets current position and invoke the appropriate events
                Stimulus lastKnowLocation = new Stimulus(gameplayInfo.Target.transform.position, Time.time, 100, SenseKind.Undefined);
                gameplayInfo.CurrentStimulus = lastKnowLocation;
                gameplayInfo.Target = null;
                GainedNewStimulus.Invoke();
                OnLostTarget.Invoke();
            }
            //Since we don't have a target we need to evaluate our current visible sight targets
            else
            {
                if (visibilityBySightTarget.Count == 0)
                    return;
                
                //Find our sight target we can see the most
                (PlayerTarget sightTarget, float value) mostVisibleTarget = (null, float.MinValue);
                foreach (KeyValuePair<PlayerTarget,float> sightPair in visibilityBySightTarget)
                {
                    if (sightPair.Value > mostVisibleTarget.value)
                    {
                        mostVisibleTarget = (sightPair.Key, sightPair.Value);
                    }
                }

                //If that target meets the visibility threshold set our new target and fire appropriate events
                if (mostVisibleTarget.value >= visibilityThreshold)
                {
                    gameplayInfo.Target = mostVisibleTarget.sightTarget;
                    gameplayInfo.CurrentStimulus = null;
                    OnGainedTarget.Invoke();
                }
                //If that target doesn't meet the visibility threshold but it does meet the notice threshold generate a new
                //stimulus at the position of the sight target
                else if (mostVisibleTarget.value > noticeThreshold)
                {
                    Stimulus noticeLocation = new Stimulus(mostVisibleTarget.sightTarget.transform.position, Time.time, mostVisibleTarget.value, SenseKind.Undefined);
                    HandleOnSensedStimulus(noticeLocation);
                }
            }
            visibilityBySightTarget.Clear();
        }

        //Wrapper for invoking the OnLostTarget event from the Visual Scripting system
        public void LostTarget()
        {
            OnLostTarget.Invoke();
            gameplayInfo.Target = null;
        }
    }
}