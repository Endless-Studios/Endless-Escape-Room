using Sight;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class holds values that change during gameplay and need to be accessed by multiple classes or visual scripting
    /// nodes. This component should be placed on the root game object of the Ai prefab. 
    /// </summary>
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