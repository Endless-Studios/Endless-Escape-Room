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
            PlayerHUD.Instance.FadeToBlack.FadeOut(fadeoutCompleteCallback, new UnityAction(LocalFadeoutCompleteCallback));
        }

        private void LocalFadeoutCompleteCallback()
        {
            playerCore.CharacterController.enabled = true;
            PlayerCore.LocalPlayer.PlayerInput.UnblockLook(this);
            PlayerCore.LocalPlayer.PlayerInput.UnblockMovement(this);
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
            playerCore.CharacterMovement.DisableCrouchToggle();
            playerCore.ItemInspector.ForceStopInspection();
            playerCore.CharacterController.enabled = false;
            PlayerCore.LocalPlayer.PlayerInput.BlockLook(this);
            PlayerCore.LocalPlayer.PlayerInput.BlockMovement(this);
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