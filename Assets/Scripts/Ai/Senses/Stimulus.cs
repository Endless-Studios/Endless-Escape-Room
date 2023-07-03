using Sight;
using Sound;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// Represents the information provided by a sense.
    /// </summary>
    public class Stimulus
    {
        public readonly Vector3 Position;
        public readonly float Value;
        public readonly float Time; 
        public readonly SenseKind SenseKind;

        public Stimulus(Vector3 position, float time, float value, SenseKind senseKind)
        {
            Position = position;
            Time = time;
            Value = value;
            SenseKind = senseKind;
        }
    }

    /// <summary>
    /// Adds the player target to the standard stimulus
    /// </summary>
    public class SightStimulus : Stimulus
    {
        public readonly PlayerTarget PlayerTarget;
        
        public SightStimulus(Vector3 position, float time, float value, PlayerTarget playerTarget) : base(position, time, value, SenseKind.Sight)
        {
            PlayerTarget = playerTarget;
        }
    }

    /// <summary>
    /// Adds the SoundType and possible PointOfInterest to the standard stimulus
    /// </summary>
    public class SoundStimulus : Stimulus
    {
        public readonly SoundType SoundType;
        public readonly PointOfInterest PointOfInterest;
        
        public SoundStimulus(Vector3 position, float time, float value, SoundType soundType, PointOfInterest pointOfInterest) : base(position, time, value, SenseKind.Hearing)
        {
            SoundType = soundType;
            PointOfInterest = pointOfInterest;
        }
    }

    /// <summary>
    /// Doesn't add any new information other than the derived class type
    /// </summary>
    public class ProximityStimulus : Stimulus
    {
        public ProximityStimulus(Vector3 position, float time, float value) : base(position, time, value, SenseKind.Proximity)
        {
            
        }
    }
}