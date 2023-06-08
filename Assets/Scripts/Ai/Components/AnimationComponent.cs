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
        [SerializeField] private string pursueTriggerName;
        [SerializeField] private string searchTriggerName;

        private int enterDoorway;
        private int moving;
        private int attack;
        private int pursue;
        private int search;

        protected void Awake()
        {
            enterDoorway = Animator.StringToHash(thresholdTriggerName);
            moving = Animator.StringToHash(movingBoolName);
            attack = Animator.StringToHash(attackTriggerName);
            pursue = Animator.StringToHash(pursueTriggerName);
            search = Animator.StringToHash(searchTriggerName);
            entity.OnWalkingThroughDoorway += WalkThroughDoor;
            entity.OnStartedAttacking.AddListener(HandleStartedAttacking);
            entity.OnStartedPursueAnimation.AddListener(HandleStartedPursueAnimation);
            entity.OnStartedSearchAnimation.AddListener(HandleStartedSearchAnimation);
        }

        private void WalkThroughDoor()
        {
            references.Animator.SetTrigger(enterDoorway);
        }
        
        private void HandleStartedAttacking()
        {
            //references.Animator.SetTrigger(attack);
            Debug.Log("Playing attack animation");
            StartCoroutine(FauxAttackSteps());
        }

        private void HandleStartedPursueAnimation()
        {
            //references.Animator.SetTrigger(pursue);
            Debug.Log("Playing pursue animation");
            StartCoroutine(FauxPursueAnimation());
        }

        private void HandleStartedSearchAnimation()
        {
            //references.Animator.SetTrigger(search);
            
            StartCoroutine(FauxSearchAnimation());
        }

        private IEnumerator FauxAttackSteps()
        {
            yield return new WaitForSeconds(2f);
            DealtDamage();
            yield return new WaitForSeconds(4f);
            FinishedAttacking();
        }

        private IEnumerator FauxPursueAnimation()
        {
            yield return new WaitForSeconds(3f);
            FinishedPursueAnimation();
        }

        private IEnumerator FauxSearchAnimation()
        {
            yield return new WaitForSeconds(3f);
            FinishedSearchAnimation();
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

        /// <summary>
        /// This method is called by an Animation event and not directly through code.
        /// </summary>
        public void FinishedPursueAnimation()
        {
            entity.FinishPursueAnimation();
        }

        /// <summary>
        /// This method is called by an Animation event and not directly through code.
        /// </summary>
        public void FinishedSearchAnimation()
        {
            entity.FinishSearchAnimation();
        }

        private void Update()
        {
            references.Animator.SetBool(moving, references.NavigationComponent.IsMoving);
        }
    }
}