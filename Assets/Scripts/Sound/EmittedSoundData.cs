using Ai;
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
        public readonly PointOfInterest SourceObject;

        public EmittedSoundData(Vector3 position, float decibelsAtSource, SoundType soundKind, PointOfInterest sourceObject = null)
        {
            Position = position;
            DecibelsAtSource = decibelsAtSource;
            SoundKind = soundKind;
            SourceObject = sourceObject;
        }
    }
}