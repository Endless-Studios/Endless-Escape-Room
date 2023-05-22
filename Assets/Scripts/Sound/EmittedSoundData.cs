using UnityEngine;

namespace Sound
{
    /// <summary>
    /// This struct defines the information associated with the animation clip being played.
    /// </summary>
    public readonly struct EmittedSoundData
    {
        public readonly Vector3 Position;
        public readonly float DecibelsAtSource;
        public readonly SoundType SoundKind;
        public readonly float ClipLength;

        public EmittedSoundData(Vector3 position, float decibelsAtSource, SoundType soundKind, float clipLength)
        {
            Position = position;
            DecibelsAtSource = decibelsAtSource;
            SoundKind = soundKind;
            ClipLength = clipLength;
        }
    }
}