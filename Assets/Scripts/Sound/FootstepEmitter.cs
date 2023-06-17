using UnityEngine;

namespace Sound
{
    /// <summary>
    /// Simple implementation that emits sound based on the CharacterController's movement to emulate footfalls.
    /// </summary>
    public class FootstepEmitter : MonoBehaviour
    {
        [SerializeField] private CharacterMovement characterMovement;
        [SerializeField] private CharacterController controller;
        [SerializeField] private float strideTime;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float footStepVolumeAtMaxSpeed;

        private float nextStepTime;

        public void Update()
        {
            if (Time.time < nextStepTime)
                return;

            float velocityMagnitude = controller.velocity.magnitude;
            float speedValue = velocityMagnitude / characterMovement.MaximumLocomotionSpeed;
            float volume = footStepVolumeAtMaxSpeed * curve.Evaluate(speedValue);

            //TODO: Add code to sample surfaces and modify the clip played and the dB of the clip based on the surface
            EmittedSoundData soundData = new EmittedSoundData(transform.position, volume, SoundType.PlayerGenerated);
            AiSound.Instance.EmitSound(soundData);
            nextStepTime = Time.time + strideTime;
        }
    }
}