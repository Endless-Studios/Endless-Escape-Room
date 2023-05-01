using UnityEngine;

namespace Sound
{
    public class SoundEmitter : MonoBehaviour
    {
        [SerializeField] private AudioClip clip;
        [SerializeField] private float audioClipDB;
        [SerializeField] private SoundManager.SoundEnum soundKind;

        public void EmitSound()
        {
            SoundManager.EmittedSoundData soundData = new(transform.position, audioClipDB, soundKind, clip.length);
            SoundManager.Instance.EmitSoundAtPosition(soundData, clip);
        }
    }
}