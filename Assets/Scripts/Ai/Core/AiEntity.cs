using System;
using UnityEngine.Events;

// ReSharper disable UnusedMember.Global

namespace Ai
{
    /// <summary>
    /// Used as a central point for managing Ai events. This class should be placed on the root object of the Ai prefab.
    /// </summary>
    public class AiEntity : AiComponent
    {
        public bool ShouldSpawnOnStart;

        public UnityEvent OnDied = new UnityEvent();
        public UnityEvent OnSpawn = new UnityEvent();
        public UnityEvent OnDespawn = new UnityEvent();
        public UnityEvent OnFinishedInteracting = new UnityEvent();
        public UnityEvent OnFinishedFidgeting = new UnityEvent();
        public UnityEvent OnStartedAttacking = new UnityEvent();
        public UnityEvent OnDealtDamage = new UnityEvent();
        public UnityEvent OnFinishedAttacking = new UnityEvent();
        public UnityEvent OnDisappear = new UnityEvent();
        public UnityEvent OnReappear = new UnityEvent();
        public UnityEvent OnStartedPursueAnimation = new UnityEvent();
        public UnityEvent OnFinishedPursueAnimation = new UnityEvent();
        public UnityEvent OnStartedSearchAnimation = new UnityEvent();
        public UnityEvent OnFinishedSearchAnimation = new UnityEvent();
        

        public event Action OnWalkingThroughDoorway;
        public event Action OnWalkedThroughDoorway;

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

        /// <summary>
        /// Invokes the OnFinishedInteracting event
        /// </summary>
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

        /// <summary>
        /// Invokes the OnFinishedFidgeting event
        /// </summary>
        public void FinishedFidgeting()
        {
            OnFinishedFidgeting.Invoke();
        }

        /// <summary>
        /// Invokes the OnStartedAttacking event
        /// </summary>
        public void StartedAttacking()
        {
            OnStartedAttacking.Invoke();
        }

        /// <summary>
        /// Invokes the OnFinishedAttacking event
        /// </summary>
        public void FinishedAttacking()
        {
            OnFinishedAttacking.Invoke();
        }

        /// <summary>
        /// Invokes the OnDealtDamage event
        /// </summary>
        public void DealtDamage()
        {
            OnDealtDamage.Invoke();
        }

        /// <summary>
        /// Invokes the OnDisappear event
        /// </summary>
        public void Disappear()
        {
            OnDisappear.Invoke();
            Invoke(nameof(Reappear), 2f);
        }

        /// <summary>
        /// Invokes the OnReappear event
        /// </summary>
        public void Reappear()
        {
            OnReappear.Invoke();
        }

        public void StartPursueAnimation()
        {
            OnStartedPursueAnimation.Invoke();
        }

        public void FinishPursueAnimation()
        {
            OnFinishedPursueAnimation.Invoke();
        }

        public void StartSearchAnimation()
        {
            OnStartedSearchAnimation.Invoke();
        }

        public void FinishSearchAnimation()
        {
            OnFinishedSearchAnimation.Invoke();
        }
        
    }
}