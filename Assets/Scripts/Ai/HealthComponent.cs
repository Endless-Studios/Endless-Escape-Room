using System;
using UnityEngine;

namespace Ai
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private float maxHealth;

        public float MaxHealth => maxHealth;
        public float Health { get; private set; }

        public event Action OnDied; 

        [ContextMenu("Take 10 Damage")]
        internal void TakeDamage10()
        {
            TakeDamage(10);
        }
    
        internal void TakeDamage(float amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                OnDied?.Invoke();
            }
        }

        public void SetHealthToMax()
        {
            Health = MaxHealth;
        }
    }
}