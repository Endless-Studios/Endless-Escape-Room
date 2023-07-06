using Ai;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// This component produces a sound for the AI to here with the given attributes.
    /// </summary>
    public class AiSoundEmitter : MonoBehaviour
    {
        [SerializeField] private PointOfInterest pointOfInterest;
        [SerializeField] private Collider lineOfSightCollider;
        [SerializeField] private float soundDecibels;
        [SerializeField] private SoundType soundKind;

        /// <summary>
        /// Emits a sound for the Ai to hear.
        /// </summary>
        [ContextMenu("Emit Sound")]
        public void EmitSound()
        {
            EmitSound(transform.position);
        }

        /// <summary>
        /// Emits a sound for the Ai to hear at a specific position.
        /// </summary>
        /// <param name="position"></param>
        public void EmitSound(Vector3 position)
        {
            EmittedSoundData soundData = new EmittedSoundData(position, soundDecibels, soundKind, pointOfInterest, lineOfSightCollider);
            AiSound.Instance.EmitSound(soundData);
        }
    }
}