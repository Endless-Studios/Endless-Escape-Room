using System;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// Manages playing sounds in the scene. Maintains a pool of audio sources and plays clips through them.
    /// </summary>
    public class SoundManager : MonoBehaviourSingleton<SoundManager>
    {
        [field: SerializeField] public float DefaultSoundBlockingValue { get; private set; }
        [field: SerializeField] public LayerMask SoundBlockerMask { get; private set; }
        public event Action<EmittedSoundData> OnSoundEmitted;

        public void EmitSoundAtPosition(EmittedSoundData soundData, AudioClip clip)
        {
            //PlayClip from pooled audio source
            OnSoundEmitted?.Invoke(soundData);
        }
    }
}