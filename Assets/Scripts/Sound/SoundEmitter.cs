using System;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// This component interacts the with the SoundManager to play clips at the attached GameObject's position
    /// </summary>
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private float audioClipDB;
        [SerializeField] private SoundType soundKind;

        [ContextMenu("Emit Sound")]
        public void EmitSound()
        {
            float clipLength = clip ? clip.length : 1f;
            EmittedSoundData soundData = new EmittedSoundData(transform.position, audioClipDB, soundKind, clipLength);
            SoundManager.Instance.EmitSoundAtPosition(soundData, clip);
        }

        public void EmitSound(AudioClip overrideClip, float clipDB)
        {
            float clipLength = overrideClip ? overrideClip.length : 1f;
            EmittedSoundData soundData = new EmittedSoundData(transform.position, clipDB, soundKind, clipLength);
            SoundManager.Instance.EmitSoundAtPosition(soundData, overrideClip);
        }

        public void EmitSound(float clipDB)
        {
            float clipLength = clip ? clip.length : 1f;
            EmittedSoundData soundData = new EmittedSoundData(transform.position, clipDB, soundKind, clipLength);
            SoundManager.Instance.EmitSoundAtPosition(soundData, clip);
        }
    }
}