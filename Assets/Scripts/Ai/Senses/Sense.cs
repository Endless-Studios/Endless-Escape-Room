using System;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// Public interface for all that should be implemented by all components that represent a sense.
    /// </summary>
    public abstract class Sense : MonoBehaviour
    {
        public event Action<Stimulus> OnSensedStimulus;

        protected void SensedStimulus(Stimulus stimulus)
        {
            OnSensedStimulus?.Invoke(stimulus);
        }
    }
}