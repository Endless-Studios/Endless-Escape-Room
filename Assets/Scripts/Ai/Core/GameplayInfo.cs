using System;
using Sight;
using UnityEngine;

namespace Ai
{
    public class GameplayInfo : MonoBehaviour
    {
        [ShowOnly] public PointOfInterest CurrentPointOfInterest;
        [ShowOnly] public Hideout TargetHideout;
        [ShowOnly] public PlayerTarget Target;
        [ShowOnly] public Hideout PlayersHideout;
        [ShowOnly] public Vector3 Destination;
        [HideInInspector] public bool ShouldSpawnInPlace;
        
        public Stimulus CurrentStimulus;
        public Vector3 InitialSpawnPoint { get; private set; }

        private void Awake()
        {
            InitialSpawnPoint = transform.position;
        }

        public void ResetValues()
        {
            CurrentPointOfInterest = null;
            TargetHideout = null;
            Target = null;
            PlayersHideout = null;
            Destination = transform.position;
            CurrentStimulus = null;
        }
    }
}