using System.Collections;
using System.Collections.Generic;
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
        [SerializeField] private string thresholdPursueTriggerName;
        [SerializeField] private string movingBoolName;
        [SerializeField] private string lowAttackTriggerName;
        [SerializeField] private string midAttackTriggerName;
        [SerializeField] private string highAttackTriggerName;
        [SerializeField] private string pursueTriggerName;
        [SerializeField] private string pursueShortTriggerName;
        [SerializeField] private string searchTriggerName;
        [SerializeField] private string visualSweepTriggerName;
        [SerializeField] private List<InteractionAnimationPair> interactionAnimationPairs = new List<InteractionAnimationPair>();
        [SerializeField] private List<string> fidgetNames;
        [SerializeField] private float lowAttackHeightDifference;
        [SerializeField] private float highAttackHeightDifference;
        [SerializeField] private string traversingThresholdName;

        private readonly Dictionary<InteractionType, string> animationNamesByInteractionType = new Dictionary<InteractionType, string>();
        
        private int enterDoorway;
        private int enterDoorwayPursue;
        private int moving;
        private int lowAttack;
        private int midAttack;
        private int highAttack;
        private int pursue;
        private int pursueShort;
        private int search;
        private int visualSweep;
        private int traversingThreshold;
        
        private int velX;
        private int velY;
        
        protected void Awake()
        {
            enterDoorway = Animator.StringToHash(thresholdTriggerName);
            enterDoorwayPursue = Animator.StringToHash(thresholdPursueTriggerName);
            moving = Animator.StringToHash(movingBoolName);
            lowAttack = Animator.StringToHash(lowAttackTriggerName);
            midAttack = Animator.StringToHash(midAttackTriggerName);
            highAttack = Animator.StringToHash(highAttackTriggerName);
            pursue = Animator.StringToHash(pursueTriggerName);
            pursueShort = Animator.StringToHash(pursueShortTriggerName);
            search = Animator.StringToHash(searchTriggerName);
            visualSweep = Animator.StringToHash(visualSweepTriggerName);
            traversingThreshold = Animator.StringToHash(traversingThresholdName);
            velX = Animator.StringToHash("VelX");
            velY = Animator.StringToHash("VelY");
            entity.OnStartedAttacking.AddListener(HandleStartedAttacking);
            entity.OnStartedPursueAnimation.AddListener(HandleStartedPursueAnimation);
            entity.OnStartedSearchAnimation.AddListener(HandleStartedSearchAnimation);
            entity.OnStartedVisualSweepAnimation.AddListener(HandleStartedVisualSweepAnimation);
            entity.OnWalkingThroughDoorway += WalkThroughDoor;
            entity.OnWalkingThroughDoorway += WalkThroughDoor;

            //Translate the list to a dictionary for easier lookup and data validation
            for (int i = 0; i < interactionAnimationPairs.Count; i++)
            {
                InteractionAnimationPair pair = interactionAnimationPairs[i];
                
                if (!animationNamesByInteractionType.ContainsKey(pair.InteractionType))
                {
                    animationNamesByInteractionType.Add(pair.InteractionType, pair.animationTriggerName);
                    continue;
                }
                
                Debug.LogWarning("Multiple animation names associated with one interaction type, only the first will be accepted");
            }
        }
        
        /// <summary>
        /// Plays the interaction animation associated with the interaction type.
        /// </summary>
        /// <param name="interactionType"></param>
        public void PlayInteractionAnimation(InteractionType interactionType)
        {
            if (!animationNamesByInteractionType.TryGetValue(interactionType, out string animationName))
            {
                Debug.LogWarning("No animation found for the given interaction type");
                return;
            }
            
            references.Animator.SetTrigger(animationName);
        }

        public void SetTraversalState(bool isTraversing)
        {
            references.Animator.SetBool(traversingThreshold, isTraversing);
        }

        /// <summary>
        /// Plays a random fidget animation from the provided list.
        /// </summary>
        public void PlayRandomFidgetAnimation()
        {
            if (fidgetNames.Count < 0)
            {
                Debug.LogWarning("No animation names found in the fidget animation list");
                return;
            }

            int index = Random.Range(0, fidgetNames.Count);
            string animationName = fidgetNames[index];
            
            references.Animator.SetTrigger(animationName);
        }

        private void WalkThroughDoor()
        {
            if(gameplayInfo.AiAwarenessState == AiAwarenessState.Pursuing) 
                references.Animator.SetTrigger(enterDoorwayPursue);
            else
                references.Animator.SetTrigger(enterDoorway);
        }

        private void HandleStartedAttacking()
        {
            float heightDifference = PlayerCore.LocalPlayer.transform.position.y - entity.transform.position.y;
            
            if (heightDifference > highAttackHeightDifference)
            {
                references.Animator.SetTrigger(highAttack);
                return;
            }

            if (heightDifference < lowAttackHeightDifference || PlayerCore.LocalPlayer.CharacterMovement.IsCrouching)
            {
                references.Animator.SetTrigger(lowAttack);
                return;
            }
            
            references.Animator.SetTrigger(midAttack);
        }

        private void HandleStartedPursueAnimation()
        {
            float distance = Vector3.Distance(gameplayInfo.Target.transform.position, entity.transform.position);
            
            if(distance > attributes.PursueAnimationThreshold)
                references.Animator.SetTrigger(pursue);
            else
                references.Animator.SetTrigger(pursueShort);
        }

        private void HandleStartedSearchAnimation()
        {
            references.Animator.SetTrigger(search);
        }

        private void HandleStartedVisualSweepAnimation()
        {
            references.Animator.SetTrigger(visualSweep);
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
        public void AiInteracted()
        {
            entity.AiInteracted();
        }

        /// <summary>
        /// This method is called by an Animation event and not directly through code.
        /// </summary>
        public void FinishedInteractionAnimation()
        {
            entity.FinishedInteractionAnimation();
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
        
        /// <summary>
        /// This method is called by an Animation event and not directly through code.
        /// </summary>
        public void FinishedFidgeting()
        {
            entity.FinishedFidgeting();
        }

        /// <summary>
        /// This method is called by an Animation event and not directly through code.
        /// </summary>
        public void FinishedVisualSweep()
        {
            entity.FinishVisualSweepAnimation();
        }
        
        private void Update()
        {
            references.Animator.SetBool(moving, references.NavigationComponent.IsMoving);
            references.Animator.SetFloat(velX, references.NavigationComponent.LocalRelativeVelocity.x);
            references.Animator.SetFloat(velY, references.NavigationComponent.LocalRelativeVelocity.z);
        }
    }
}