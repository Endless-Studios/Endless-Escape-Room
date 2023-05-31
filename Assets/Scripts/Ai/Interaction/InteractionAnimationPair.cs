using System;

namespace Ai
{
    /// <summary>
    /// A pair of values that relates an Interaction Type with a string that represents the name
    /// of an animation trigger parameter.
    /// </summary>
    [Serializable]
    public struct InteractionAnimationPair
    {
        public InteractionType InteractionType;
        public string animationTriggerName;
    }
}