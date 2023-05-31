using System;

namespace Ai
{
    /// <summary>
    /// Public interface for all that should be implemented by all components that represent a sense.
    /// </summary>
    public interface ISense
    {
        public event Action<Stimulus> OnSensedStimulus;
    }
}