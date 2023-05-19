using System;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class manages pushing information directly into the Animator for animating the ai. This class should be
    /// attached to the same GameObject that has the Animator component and should be referenced by the AiReferences
    /// component.
    /// </summary>
    public class AnimationComponent : AiComponent
    {
        [SerializeField] private Animator animator;

        [SerializeField] private string thresholdTriggerName;
        [SerializeField] private string movingBoolName;

        private int enterDoorway;
        private int moving;

        protected void Awake()
        {
            enterDoorway = Animator.StringToHash(thresholdTriggerName);
            moving = Animator.StringToHash(movingBoolName);
            Entity.OnWalkingThroughDoorway += WalkThroughDoor;
        }

        private void WalkThroughDoor()
        {
            animator.SetTrigger(enterDoorway);
        }

        /// <summary>
        /// This method is called by an Animation event and not directly through code. 
        /// </summary>
        public void WalkedThroughDoor()
        {
            Entity.WalkedThroughDoorway();
            transform.localPosition = Vector3.zero;
        }

        
        private void Update()
        {
            animator.SetBool(moving, references.NavigationComponent.IsMoving);
        }
    }
}