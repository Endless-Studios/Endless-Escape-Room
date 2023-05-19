using UnityEngine;

namespace Ai
{
    public class AiIk : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        public GameObject lookAtObject;
        
        private void OnAnimatorIK(int layerIndex)
        {
            if (animator)
            {
                animator.SetLookAtWeight(1);
                animator.SetLookAtPosition(lookAtObject.transform.position);
            }
        }
    }
}