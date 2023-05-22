using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable UnusedMember.Global

namespace Ai
{
    /// <summary>
    /// Used as a central point for managing Ai state. This component Should be placed on the root object of the Ai prefab.
    /// </summary>
    public class AiEntity : MonoBehaviour
    {
        public bool ShouldSpawnOnStart;

        public UnityEvent OnDied = new UnityEvent();
        public UnityEvent OnSpawn = new UnityEvent();
        public UnityEvent OnDespawn = new UnityEvent();

        public event Action OnWalkingThroughDoorway;
        public event Action OnWalkedThroughDoorway;
        
        public UnityEvent<PointOfInterest> OnStartedInteracting;
        public UnityEvent OnFinishedInteracting;

        /// <summary>
        /// Invokes the OnWalkingThroughDoorway event
        /// </summary>
        public void WalkingThroughThreshold()
        {
            OnWalkingThroughDoorway?.Invoke();
        }
        
        /// <summary>
        /// Invokes the OnWalkedThroughDoorway event
        /// </summary>
        public void WalkedThroughDoorway()
        {
            OnWalkedThroughDoorway?.Invoke();
        }

        public void StartedInteracting(PointOfInterest pointOfInterest)
        {
            OnStartedInteracting?.Invoke(pointOfInterest);
        }

        public void FinishedInteracting()
        {
            OnFinishedInteracting?.Invoke();
        }

        /// <summary>
        /// Invokes the OnSpawn event
        /// </summary>
        public void Spawn()
        {
            OnSpawn.Invoke();
        }

        /// <summary>
        /// Invokes the OnDespawn event
        /// </summary>
        public void Despawn()
        {
            OnDespawn.Invoke();
        }

        /// <summary>
        /// Invokes the OnDied event
        /// </summary>
        public void Died()
        {
            OnDied.Invoke();
        }
    }
}