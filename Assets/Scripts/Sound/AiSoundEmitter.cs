using UnityEngine;

namespace Sound
{
    /// <summary>
    /// This component interacts the with the SoundManager to play clips at the attached GameObject's position
    /// </summary>
    public class AiSoundEmitter : MonoBehaviour
    {
        [SerializeField] private float soundDecibels;
        [SerializeField] private SoundType soundKind;

        [ContextMenu("Emit Sound")]
        public void EmitSound()
        {
            EmittedSoundData soundData = new EmittedSoundData(transform.position, soundDecibels, soundKind, gameObject);
            AiSound.Instance.EmitSound(soundData);
        }

        public void EmitSound(Vector3 position)
        {
            EmittedSoundData soundData = new EmittedSoundData(position, soundDecibels, soundKind, gameObject);
            AiSound.Instance.EmitSound(soundData);
        }
    }
}