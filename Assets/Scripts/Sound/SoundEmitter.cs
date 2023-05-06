using System;
using UnityEngine;

namespace Sound
{
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private float audioClipDB;
        [SerializeField] private SoundManager.SoundEnum soundKind;

        [ContextMenu("Emit Sound")]
        public void EmitSound()
        {
            float clipLength = clip ? clip.length : 1f;
            SoundManager.EmittedSoundData soundData = new(transform.position, audioClipDB, soundKind, clipLength);
            SoundManager.Instance.EmitSoundAtPosition(soundData, clip);
        }

        public void EmitSound(AudioClip overrideClip, float clipDB)
        {
            float clipLength = overrideClip ? overrideClip.length : 1f;
            SoundManager.EmittedSoundData soundData = new(transform.position, clipDB, soundKind, clipLength);
            SoundManager.Instance.EmitSoundAtPosition(soundData, overrideClip);
        }

        public void EmitSound(float clipDB)
        {
            float clipLength = clip ? clip.length : 1f;
            SoundManager.EmittedSoundData soundData = new(transform.position, clipDB, soundKind, clipLength);
            SoundManager.Instance.EmitSoundAtPosition(soundData, clip);
        }
    }
}