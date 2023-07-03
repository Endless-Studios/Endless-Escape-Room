using System.Collections;
using Ai;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// Repeatedly emits a sound the Ai can hear with the given attributes and period between emissions
    /// </summary>
    public class AiSoundRepeatingEmitter : MonoBehaviour
    {
        [SerializeField] private PointOfInterest pointOfInterest;
        [SerializeField] private float soundDecibels;
        [SerializeField] private SoundType soundKind;
        [SerializeField] private float period;

        private Coroutine emitSoundRoutine;

        /// <summary>
        /// Starts emitting a sound continuously
        /// </summary>
        public void StartEmittingSound()
        {
            if(emitSoundRoutine is not null)
                return;
            
            emitSoundRoutine = StartCoroutine(EmitSoundRoutine());
        }

        /// <summary>
        /// Stops emitting sound.
        /// </summary>
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