using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Ai
{
    /// <summary>
    /// This class represents a special kind of point of interest that may hide a player. This
    /// class provides additional functionality to the Ai so they can determine if and how to find the
    /// player.
    /// </summary>
    public class Hideout : PointOfInterest
    {
        /// <summary>
        /// A static list of all hideouts that each Ai adds to or removes itself from. 
        /// </summary>
        public static readonly List<Hideout> Hideouts = new List<Hideout>();
        public static event Action<Hideout> OnEnteredHideout;
        public static event Action<Hideout> OnLeftHideout;

        public UnityEvent OnEnterHideout;
        public UnityEvent OnExitHideout;
        
        public UnityEvent OnForcedExit;

        protected override void OnEnable()
        {
            base.OnEnable();
            Hideouts.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Hideouts.Remove(this);
        }

        /// <summary>
        /// Public invoker for On Entered Hideout.
        /// </summary>
        public void PlayerEnteredHideout()
        {
            PlayerCore.LocalPlayer.HidingComponent.StartHiding();
            PlayerCore.LocalPlayer.PlayerTarget.enabled = false;
            OnEnteredHideout?.Invoke(this);
            OnEnterHideout.Invoke();
        }

        /// <summary>
        /// Public invoker for On Left Hideout.
        /// </summary>
        public void PlayerLeftHideout()
        {
            PlayerCore.LocalPlayer.HidingComponent.StopHiding();
            PlayerCore.LocalPlayer.PlayerTarget.enabled = true;
            OnLeftHideout?.Invoke(this);
            OnExitHideout.Invoke();
        }

        /// <summary>
        /// Called from the AI when it starts kicking the player out
        /// </summary>
        public void KickedOut()
        {
            OnForcedExit.Invoke();
        }
    }
}