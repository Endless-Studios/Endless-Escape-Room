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

        [SerializeField] List<LosProbe> losProbes = new List<LosProbe>();
        [SerializeField] PlayerCore playerCore = null;

        public List<LosProbe> LosProbes => losProbes;
        public PlayerCore PlayerCore => playerCore;

        private void OnEnable()
        {
            SenseTargets.Add(this);
        }

        private void OnDisable()
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

        ///<summary>
        /// AI deals damage to the player.
        ///</summary>
        public void DealDamage(float damage)
        {
            PlayerCore.HealthComponent.TakeDamage(damage);
        }

        /// <summary>
        /// Called at the beginning of an AI attack
        /// </summary>
        public void HandleAttacked()
        {
            //Force exit of any camera focus
            playerCore.CameraManager.FocusCamera(null);

            //TODO Cancel any inspection or similar behaviors
        }

        /// <summary>
        /// Called when an AI needs to snap the Player to a position
        /// </summary>
        public void SnapToPosition(Vector3 position)
        {
            playerCore.Rigidbody.position = position;
        }
    }
}