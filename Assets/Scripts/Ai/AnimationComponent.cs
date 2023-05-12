using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class should be attached to the same GameObject that has the Animator component and should be referenced by
    /// the AiFacade. This class manages pushing information directly into the Animator for animating the ai.
    /// </summary>
    internal class AnimationComponent : AiComponent
    {
        [SerializeField] private Animator animator;
        
        private static readonly int EnterDoorway = Animator.StringToHash("EnterDoorway");
        private static readonly int Moving = Animator.StringToHash("Moving");

        protected void Awake()
        {
            facade.OnWalkingThroughDoorway += WalkThroughDoor;
        }

        private void WalkThroughDoor()
        {
            animator.SetTrigger(EnterDoorway);
        }

        /// <summary>
        /// This method is called by an Animation event and not directly through code. 
        /// </summary>
        public void WalkedThroughDoor()
        {
            facade.WalkedThroughDoorway();
            transform.localPosition = Vector3.zero;
        }

        
        private void Update()
        {
            animator.SetBool(Moving, facade.IsMoving);
        }
    }
}