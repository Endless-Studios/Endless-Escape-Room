using Sight;
using UnityEngine;

namespace Ai
{
    public class Stimulus
    {
        public readonly Vector3 Position;
        public float Value;
        public readonly float Time; 
        public readonly SenseKind SenseKind;
        public readonly SenseTarget SenseTarget;

        public Stimulus(Vector3 position, float time, float value, SenseKind senseKind, SenseTarget senseTarget = null)
        {
            Position = position;
            Time = time;
            Value = value;
            SenseKind = senseKind;
            SenseTarget = senseTarget;
        }
    }
}