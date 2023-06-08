using System.Collections;
using UnityEngine;

namespace Sound
{
    public class AiSoundRepeatingEmitter : MonoBehaviour
    {
        [SerializeField] private float soundDecibels;
        [SerializeField] private SoundType soundKind;
        [SerializeField] private float period;

        private Coroutine emitSoundRoutine;

        public void StartEmittingSound()
        {
            if(emitSoundRoutine is not null)
                return;
            
            EmittedSoundData soundData = new EmittedSoundData(transform.position, soundDecibels, soundKind);
            AiSound.Instance.EmitSound(soundData);
            emitSoundRoutine = StartCoroutine(EmitSoundRoutine());
        }

        public void StopEmittingSound()
        {
            if (emitSoundRoutine is not null)
                return;
            
            StopCoroutine(emitSoundRoutine);
            emitSoundRoutine = null;
        }

        private IEnumerator EmitSoundRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(period);
                EmittedSoundData soundData = new EmittedSoundData(transform.position, soundDecibels, soundKind);
                AiSound.Instance.EmitSound(soundData);
            }
        }
    }
}