using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// Manages playing sounds in the scene. Maintains a pool of audio sources and plays clips through them.
    /// </summary>
    public class SoundPoolManager : MonoBehaviourSingleton<SoundPoolManager>
    {
        [field: SerializeField] public AudioSource AudioSourcePrefab { get; private set; }

        private List<AudioSource> audioSourcePool = new List<AudioSource>();

        public void PlaySoundAtPosition(Vector3 position, AudioClip clip, float volume = 1)
        {
            //PlayClip from pooled audio source
            AudioSource audioSource = GetAvailableAudioSource();
            audioSource.transform.position = position;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.Play();

            //Schedule pooling
            StartCoroutine(AddAudioSourceToPoolAfterDelay(audioSource, clip.length));
        }

        private AudioSource GetAvailableAudioSource()
        {
            AudioSource useAudioSource = null;

            //check pool for an available audio source
            if(audioSourcePool.Count > 0)
            {
                useAudioSource = audioSourcePool[0];
                audioSourcePool.RemoveAt(0);
            }
            else
            {
                //no audio source available, create one
                useAudioSource = CreateAudioSoruce(); ;
            }

            return useAudioSource;
        }

        private AudioSource CreateAudioSoruce()
        {
            AudioSource createdAudioSource = GameObject.Instantiate<AudioSource>(AudioSourcePrefab);
            return createdAudioSource;
        }

        private IEnumerator AddAudioSourceToPoolAfterDelay(AudioSource targetAudioSource, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            audioSourcePool.Add(targetAudioSource);
        }
    }
}