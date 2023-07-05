using Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ai
{
    /// <summary>
    /// Plays audio clips when the Ai steps.
    /// </summary>
    public class AiFootstepEmitter : AiComponent
    {
        [SerializeField] private AudioClip[] footstepAudioClips;
        [SerializeField] private AnimationCurve stepVolumeCurve;
        [SerializeField] private Transform emitTransform;

        private void Awake()
        {
            entity.OnFootstep.AddListener(HandleOnFootstep);
        }

        private void HandleOnFootstep()
        {
            float normalizedSpeed = references.NavigationComponent.LocalRelativeVelocity.magnitude;
            if (references.NavigationComponent.IsTraversingLink)
            {
                if (gameplayInfo.AnimationTraversalType == AnimationTraversalType.Pursuing)
                    normalizedSpeed = 1;
                else
                    normalizedSpeed = .5f;
            }
            AudioClip clip = footstepAudioClips[Random.Range(0, footstepAudioClips.Length)];
            SoundPoolManager.Instance.PlaySoundAtPosition(emitTransform.position, clip, stepVolumeCurve.Evaluate(normalizedSpeed));
        }
    }
}