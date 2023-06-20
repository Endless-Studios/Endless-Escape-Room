using System.Collections.Generic;
using Sight;
using UnityEngine;
using UnityEngine.Events;

namespace Ai
{
    /// <summary>
    /// This component is how the Ai senses track the player and how the Ai affects the player. 
    /// </summary>
    public class PlayerTarget : MonoBehaviour
    {
        public static readonly List<PlayerTarget> SenseTargets = new List<PlayerTarget>();

        [field: SerializeField] public List<LosProbe> LosProbes { get; private set; }
        [field: SerializeField] public HealthComponent HealthComponent { get; private set; }

        private void Awake()
        {
            SenseTargets.Add(this);
        }

        private void OnDestroy()
        {
            SenseTargets.Remove(this);
        }


        ///<summary>
        /// AI triggers a fadeout on the player.
        ///</summary>
        public void StartFadeout(UnityAction fadeoutCompleteCallback)
        {
            PlayerHUD.Instance.FadeToBlack.FadeOut(fadeoutCompleteCallback);
        }

        /// <summary>
        /// Ai stops the player from moving or looking around while attacking or grabbing the player out of the hideout.
        /// </summary>
        public void DisableInput()
        {
            //TODO: Implement
        }

        /// <summary>
        /// Ai reenables player input after finishing its attack.
        /// </summary>
        public void EnableInput()
        {
            //TODO: Implement
        }

        ///<summary>
        /// AI deals damage to the player.
        ///</summary>
        public void DealDamage(float damage)
        {
            HealthComponent.TakeDamage(damage);
        }
    }
}