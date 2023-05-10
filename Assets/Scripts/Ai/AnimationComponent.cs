using UnityEngine;

namespace Ai
{
    internal class AnimationComponent : AiComponent
    {
        [SerializeField] private Animator animator;
        
        private static readonly int EnterDoorway = Animator.StringToHash("EnterDoorway");
        private static readonly int Moving = Animator.StringToHash("Moving");

        protected override void Awake()
        {
            base.Awake();
            facade.OnWalkingThroughDoorway += WalkThroughDoor;
        }

        private void WalkThroughDoor()
        {
            animator.SetTrigger(EnterDoorway);
        }

        public void WalkedThroughDoor()
        {
            Debug.Log("WalkedThroughDoor");
            facade.WalkedThroughDoorway();
            transform.localPosition = Vector3.zero;
        }

        public void Update()
        {
            animator.SetBool(Moving, facade.IsMoving);
        }
    }
}