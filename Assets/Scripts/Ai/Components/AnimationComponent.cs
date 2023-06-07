using System;
using System.Collections;
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
        [SerializeField] private string thresholdTriggerName;
        [SerializeField] private string movingBoolName;
        [SerializeField] private string attackTriggerName;

        private int enterDoorway;
        private int moving;
        private int attack;

        protected void Awake()
        {
            enterDoorway = Animator.StringToHash(thresholdTriggerName);
            moving = Animator.StringToHash(movingBoolName);
            attack = Animator.StringToHash(attackTriggerName);
            entity.OnWalkingThroughDoorway += WalkThroughDoor;
            entity.OnStartedAttacking.AddListener(HandleStartedAttacking);
        }

        private void WalkThroughDoor()
        {
            references.Animator.SetTrigger(enterDoorway);
        }
        
        private void HandleStartedAttacking()
        {
            references.Animator.SetTrigger(attack);
            StartCoroutine(FauxAttackSteps());
        }

        private IEnumerator FauxAttackSteps()
        {
            yield return new WaitForSeconds(2f);
            DealtDamage();
            yield return new WaitForSeconds(4f);
            FinishedAttacking();
        }

        /// <summary>
        /// This method is called by an Animation event and not directly through code. 
        /// </summary>
        public void WalkedThroughDoor()
        {
            entity.WalkedThroughDoorway();
            transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// This method is called by an Animation event and not directly through code.
        /// </summary>
        public void FinishedInteracting()
        {
            entity.FinishedInteracting();
        }

        /// <summary>
        /// This method is called by an Animation event and not directly through code.
        /// </summary>
        public void FinishedAttacking()
        {
            entity.FinishedAttacking();
        }

        /// <summary>
        /// This method is called by an Animation event and not directly through code.
        /// </summary>
        public void DealtDamage()
        {
            entity.DealtDamage();
        }
        
        private void Update()
        {
            references.Animator.SetBool(moving, references.NavigationComponent.IsMoving);
        }
    }
}