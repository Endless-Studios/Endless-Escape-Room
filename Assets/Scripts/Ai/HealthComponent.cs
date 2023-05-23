using UnityEngine;
using UnityEngine.Events;

namespace Ai
{
    /// <summary>
    /// This component manages the Health of the object it is placed on. 
    /// </summary>
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField] private float maxHealth;

        public float MaxHealth => maxHealth;
        public float Health { get; private set; }

        public UnityEvent OnDied = new UnityEvent();

        /// <summary>
        /// Context menu method for testing only.
        /// </summary>
        [ContextMenu("Take 10 Damage")]
        public void TakeDamage10()
        {
            TakeDamage(10);
        }
    
        /// <summary>
        /// Sets the value of Health to its current value minus the amount of damage. Will invoke the OnDied event if
        /// the value of Health zero or less. 
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(float amount)
        {
            Health -= amount;
            if (Health <= 0)
            {
                OnDied?.Invoke();
            }
        }

        /// <summary>
        /// Set the value of Health to MaxHealth.
        /// </summary>
        public void SetHealthToMax()
        {
            Health = MaxHealth;
        }
    }
}