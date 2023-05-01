using System;
using UnityEngine;

namespace Sound
{
    public class SoundManager : MonoBehaviourSingleton<SoundManager>
    {
        public static event Action<EmittedSoundData> OnSoundEmitted;

        private static void SoundEmitted(EmittedSoundData soundData) => OnSoundEmitted?.Invoke(soundData);

        public void EmitSoundAtPosition(EmittedSoundData soundData, AudioClip clip)
        {
            //PlayClip from pooled audiosource
            SoundEmitted(soundData);
        }

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

        public enum SoundEnum
        {
            //These could be more granular if the behaviour doesn't match expectations
            AiGenerated,
            Prop,
            PlayerGenerated
        }
    }
}