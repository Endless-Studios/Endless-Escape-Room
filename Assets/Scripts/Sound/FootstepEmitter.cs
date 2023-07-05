using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sound
{
    /// <summary>
    /// Simple implementation that emits sound based on the CharacterController's movement to emulate footfalls.
    /// </summary>
    public class FootstepEmitter : MonoBehaviour
    {
        [SerializeField] private CharacterMovement characterMovement;
        [SerializeField] private CharacterController controller;
        [SerializeField] private float strideTimeAtMaxSpeed;
        [SerializeField] private AnimationCurve strideTimeCurve;
        [SerializeField] private float footStepDecibelsMaxSpeed;
        [SerializeField] private AnimationCurve speedDecibelsCurve;
        [SerializeField] AudioClip[] walkingFootSteps;
        [SerializeField] AudioClip[] runningFootSteps;
        [SerializeField] Transform emitLocation;

        public IEnumerator Start()
        {
            float elapsedTime = 0;
            float speed = 0;
            float speedPercentage = 0;

            while(true)
            {
                while(elapsedTime < strideTimeAtMaxSpeed)
                {//Wait until enough time has passed while moving before playing the sound
                    if(characterMovement.IsGrounded)
                    {
                        speed = controller.velocity.magnitude;
                        speedPercentage = speed / characterMovement.MaximumLocomotionSpeed;
                        elapsedTime += Time.deltaTime * strideTimeCurve.Evaluate(speedPercentage);
                    }
                    yield return null;
                }

                float finalAiDecibles = footStepDecibelsMaxSpeed * speedDecibelsCurve.Evaluate(speedPercentage);

                //TODO: Add code to sample surfaces and modify the clip played and the dB of the clip based on the surface
                EmittedSoundData soundData = new EmittedSoundData(emitLocation.position, finalAiDecibles, SoundType.PlayerGenerated);
                AiSound.Instance.EmitSound(soundData);

                AudioClip audioClipToUse = null;
                if(speed > characterMovement.WalkSpeed)
                    audioClipToUse = runningFootSteps[Random.Range(0, runningFootSteps.Length)];
                else
                    audioClipToUse = walkingFootSteps[Random.Range(0, walkingFootSteps.Length)];

                SoundPoolManager.Instance.PlaySoundAtPosition(emitLocation.position, audioClipToUse, speedDecibelsCurve.Evaluate(speedPercentage));
                elapsedTime -= strideTimeAtMaxSpeed;
            }
        }
    }
}