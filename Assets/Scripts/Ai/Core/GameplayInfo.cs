using Sight;
using UnityEngine;

namespace Ai
{
    public class GameplayInfo : MonoBehaviour
    {
        [ShowOnly] public PointOfInterest CurrentPointOfInterest;
        [ShowOnly] public Hideout TargetHideout;
        [ShowOnly] public PlayerTarget Target;
        [ShowOnly] public Hideout PlayersHideout;
        [ShowOnly] public Vector3 Destination;
        public Stimulus CurrentStimulus;
    }
}