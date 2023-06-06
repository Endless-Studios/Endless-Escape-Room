using System;
using System.Collections;
using System.Collections.Generic;
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
        public static event Action<EmittedSoundData> OnSoundEmitted;

        [field: SerializeField] public AudioSource AudioSourcePrefab { get; private set; }

        private List<AudioSource> audioSourcePool = new List<AudioSource>();

        public void EmitSoundAtPosition(EmittedSoundData soundData, AudioClip clip)
        {
            //PlayClip from pooled audio source
            AudioSource useAudioSource = GetAvailableAudioSource();
            useAudioSource.transform.position = soundData.Position;
            useAudioSource.clip = clip;
            useAudioSource.Play();

            //Schedule pooling
            StartCoroutine(AddAudioSourceToPoolAfterDelay(useAudioSource, soundData.ClipLength));

            OnSoundEmitted?.Invoke(soundData);
        }

        private AudioSource GetAvailableAudioSource()
        {
            AudioSource useAudioSource = null;

            //check pool for an available audio source
            for (int i = 0; i < audioSourcePool.Count; i++)
            {
                if (audioSourcePool[i].isPlaying == false)
                {
                    useAudioSource = audioSourcePool[i];
                    audioSourcePool.Remove(useAudioSource);
                    break;
                }
            }

            //no audio source available, create one
            if (useAudioSource == null)
                useAudioSource = CreateAudioSoruce();

            return useAudioSource;
        }

        private AudioSource CreateAudioSoruce()
        {
            AudioSource createdAudioSource = GameObject.Instantiate<AudioSource>(AudioSourcePrefab);
            return createdAudioSource;
        }

        IEnumerator AddAudioSourceToPoolAfterDelay(AudioSource targetAudioSource, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            audioSourcePool.Add(targetAudioSource);
        }
    }
}