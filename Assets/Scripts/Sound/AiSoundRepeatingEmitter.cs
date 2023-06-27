using System.Collections;
using Ai;
using UnityEngine;

namespace Sound
{
    public class AiSoundRepeatingEmitter : MonoBehaviour
    {
        [SerializeField] private PointOfInterest pointOfInterest;
        [SerializeField] private float soundDecibels;
        [SerializeField] private SoundType soundKind;
        [SerializeField] private float period;

        private Coroutine emitSoundRoutine;

        public void StartEmittingSound()
        {
            if(emitSoundRoutine is not null)
                return;
            
            emitSoundRoutine = StartCoroutine(EmitSoundRoutine());
        }

        public void StopEmittingSound()
        {
            if (emitSoundRoutine is null)
                return;
            
            StopCoroutine(emitSoundRoutine);
            emitSoundRoutine = null;
        }

        private IEnumerator EmitSoundRoutine()
        {
            while (true)
            {
                EmittedSoundData soundData = new EmittedSoundData(transform.position, soundDecibels, soundKind, pointOfInterest);
                AiSound.Instance.EmitSound(soundData);
                yield return new WaitForSeconds(period);
            }
        }
    }
}