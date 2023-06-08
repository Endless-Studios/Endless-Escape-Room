using System;
using Sight;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class holds values that change during gameplay, are set by the Ai for implementing its behavior, and need to be accessed by multiple classes or visual scripting
    /// nodes. This component should be placed on the root game object of the Ai prefab. 
    /// </summary>
    public class GameplayInfo : MonoBehaviour
    {
        [ShowOnly] public PointOfInterest CurrentPointOfInterest;
        [ShowOnly] public Hideout TargetHideout;
        [ShowOnly] public PlayerTarget Target;
        [ShowOnly] public Hideout PlayersHideout;
        [ShowOnly] public Vector3 Destination;
        [ShowOnly] public AlertState AlertState;
        [HideInInspector] public bool ShouldSpawnInPlace;
        
        public Stimulus CurrentStimulus;
        public Vector3 InitialSpawnPoint { get; private set; }

        private void Awake()
        {
            InitialSpawnPoint = transform.position;
        }

        /// <summary>
        /// Resets all Gameplay values to their defaults. 
        /// </summary>
        public void ResetValues()
        {
            CurrentPointOfInterest = null;
            TargetHideout = null;
            Target = null;
            PlayersHideout = null;
            Destination = transform.position;
            CurrentStimulus = null;
            AlertState = AlertState.Unaware;
        }
    }
}