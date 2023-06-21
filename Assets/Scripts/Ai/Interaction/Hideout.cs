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
        
        protected override void Awake()
        {
            base.Awake();
            Hideouts.Add(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Hideouts.Remove(this);
        }

        /// <summary>
        /// Public invoker for On Entered Hideout.
        /// </summary>
        public void PlayerEnteredHideout()
        {
            PlayerCore.LocalPlayer.CharacterController.enabled = false;
            PlayerCore.LocalPlayer.NavMeshObstacle.enabled = false;
            OnEnteredHideout?.Invoke(this);
            OnEnterHideout.Invoke();
        }

        /// <summary>
        /// Public invoker for On Left Hideout.
        /// </summary>
        public void PlayerLeftHideout()
        {
            PlayerCore.LocalPlayer.CharacterController.enabled = true;
            PlayerCore.LocalPlayer.NavMeshObstacle.enabled = true;
            OnLeftHideout?.Invoke(this);
            OnExitHideout.Invoke();
        }
    }
}