using System;
using UnityEngine;

namespace Sound
{
    public class AiSound : MonoBehaviourSingleton<AiSound>
    {
        public event Action<EmittedSoundData> OnSoundEmitted;
        [SerializeField] private float defaultSoundBlockingValue;
        [SerializeField] private LayerMask soundBlockerMask;
        
        public float DefaultSoundBlockingValue => defaultSoundBlockingValue;
        public LayerMask SoundBlockerMask => soundBlockerMask;

        public void EmitSound(EmittedSoundData soundData)
        {
            OnSoundEmitted?.Invoke(soundData);
        }
    }
}