using UnityEngine;

namespace Sound
{
    public class FootstepEmitter : MonoBehaviour
    {
        [SerializeField] private CharacterController controller;
        [SerializeField] private SoundEmitter emitter;
        [SerializeField] private float strideTime;

        public void Update()
        {
            if (Time.time < nextStepTime)
                return;

            float velocityMagnitude = controller.velocity.magnitude;
            
            if (velocityMagnitude < .25f) 
                return;
            
            //TODO: Add code to sample surfaces and modify the clip played and the dB of the clip based on the surface
            
            emitter.EmitSound(60 * (velocityMagnitude / 3));
            nextStepTime = Time.time + strideTime;
        }

        private float nextStepTime;
    }
}