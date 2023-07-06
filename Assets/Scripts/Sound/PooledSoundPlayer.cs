using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Plays a sound clip using a pooled AudioSource via SoundPoolManager.
/// Best used with shorter sound clips that dont loop. For longer/looping clips use an AudioSource in scene. 
/// <summary>
public class PooledSoundPlayer : MonoBehaviour
{
    [SerializeField] private Transform soundPositionTransform;
    [SerializeField] private AudioClip[] soundClips;

    public UnityEvent<Vector3> OnSoundPlayedAtPosition = new UnityEvent<Vector3>();

    private void OnValidate()
    {
        if(soundPositionTransform == null)
            soundPositionTransform = transform;
    }

    public void PlaySound()
    {
        Sound.SoundPoolManager.Instance.PlaySoundAtPosition(soundPositionTransform.position, soundClips[Random.Range(0, soundClips.Length)]);
        OnSoundPlayedAtPosition.Invoke(soundPositionTransform.position);
    }
}