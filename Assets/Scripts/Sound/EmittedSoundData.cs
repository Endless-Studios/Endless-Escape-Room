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
        public readonly SoundEnum SoundKind;
        public readonly float ClipLength;

        public EmittedSoundData(Vector3 position, float decibelsAtSource, SoundEnum soundKind, float clipLength)
        {
            Position = position;
            DecibelsAtSource = decibelsAtSource;
            SoundKind = soundKind;
            ClipLength = clipLength;
        }
    }
}