using UnityEngine;

namespace Sound
{
    /// <summary>
    /// This struct defines the information associated with the animation clip being played.
    /// </summary>
    public readonly struct EmittedSoundData
    {
        public readonly Vector3 Position;
        public readonly float DBAtSource;
        public readonly SoundEnum SoundKind;
        public readonly float ClipLength;

        public EmittedSoundData(Vector3 position, float dbAtSource, SoundEnum soundKind, float clipLength)
        {
            Position = position;
            DBAtSource = dbAtSource;
            SoundKind = soundKind;
            ClipLength = clipLength;
        }
    }
}