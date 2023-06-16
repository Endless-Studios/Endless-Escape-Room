using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
        [SerializeField] private List<InteractionAnimationPair> interactionAnimationPairs = new List<InteractionAnimationPair>();
        [SerializeField] private List<string> fidgetNames;

        private readonly Dictionary<InteractionType, string> animationNamesByInteractionType = new Dictionary<InteractionType, string>();
        
        private int enterDoorway;
        private int moving;
        private int attack;
        private int velX;
        private int velY;
        
        protected void Awake()
        {
            enterDoorway = Animator.StringToHash(thresholdTriggerName);
            moving = Animator.StringToHash(movingBoolName);
            attack = Animator.StringToHash(attackTriggerName);
            velX = Animator.StringToHash("VelX");
            velY = Animator.StringToHash("VelY");
            entity.OnWalkingThroughDoorway += WalkThroughDoor;
            entity.OnStartedAttacking.AddListener(HandleStartedAttacking);
            
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
            references.Animator.SetTrigger(enterDoorway);
        }
        
        private void HandleStartedAttacking()
        {
            references.Animator.SetTrigger(attack);
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
        public void FinishedFidgeting()
        {
            entity.FinishedFidgeting();
        }
        
        private void Update()
        {
            references.Animator.SetBool(moving, references.NavigationComponent.IsMoving);
            references.Animator.SetFloat(velX, references.NavigationComponent.LocalRelativeVelocity.x);
            references.Animator.SetFloat(velY, references.NavigationComponent.LocalRelativeVelocity.z);
        }
    }
}