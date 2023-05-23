using System;

namespace Ai
{
    public interface ISense
    {
        public event Action<Stimulus> OnSensedStimulus;
    }
}