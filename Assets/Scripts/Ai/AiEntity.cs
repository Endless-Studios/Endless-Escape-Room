using System;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable UnusedMember.Global

namespace Ai
{
    public class AiEntity : MonoBehaviour
    {
        public bool ShouldSpawnOnStart;

        public UnityEvent OnDied;
        public UnityEvent OnSpawn;
        public UnityEvent OnDespawn;

        public event Action OnWalkingThroughDoorway;
        public event Action OnWalkedThroughDoorway;

        public void WalkingThroughDoorway()
        {
            OnWalkingThroughDoorway?.Invoke();
        }
        
        public void WalkedThroughDoorway()
        {
            OnWalkedThroughDoorway?.Invoke();
        }

        public void Spawn()
        {
            OnSpawn?.Invoke();
        }

        public void Despawn()
        {
            OnDespawn?.Invoke();
        }

        public void Died()
        {
            OnDied?.Invoke();
        }
        
    }
}