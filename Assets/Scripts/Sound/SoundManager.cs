using System;
using UnityEngine;

namespace Sound
{
    public class SoundManager : MonoBehaviourSingleton<SoundManager>
    {
        [field: SerializeField] public float DefaultSoundBlockingValue { get; private set; }
        [field: SerializeField] public LayerMask SoundBlockerMask { get; private set; }
        public static event Action<EmittedSoundData> OnSoundEmitted;

        public void EmitSoundAtPosition(EmittedSoundData soundData, AudioClip clip)
        {
            //PlayClip from pooled audiosource
            OnSoundEmitted?.Invoke(soundData);
        }
    }
}