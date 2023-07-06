using UnityEngine;
using UnityEngine.Events;

namespace Sound
{
    /// <summary>
    /// A toggleable sound player.
    /// </summary>
    public class ContinuousSoundPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        
        public UnityEvent OnStartedPlaying = new UnityEvent();
        public UnityEvent OnStoppedPlaying = new UnityEvent();
        
        public void TogglePlayingState()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                OnStoppedPlaying.Invoke();
            }
            else
            {
                audioSource.Play();
                OnStartedPlaying.Invoke();
            }
        }
    }
}