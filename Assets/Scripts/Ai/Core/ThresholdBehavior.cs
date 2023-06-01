using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ai
{
    /// <summary>
    /// This class manages a generalized value that increases over time and reaches some threshold.
    /// Its members all for randomization of the exact value of the threshold and contain information
    /// about if the value has exceeded its threshold and if it has by how much.
    /// </summary>
    public class ThresholdBehavior : MonoBehaviour
    {
        [SerializeField] private string thresholdName;
        [SerializeField] private float baseThreshold;
        [SerializeField] private float minThresholdModifier;
        [SerializeField] private float maxThresholdModifier;
        [ShowOnly, SerializeField] private float value;
        [ShowOnly, SerializeField] private float currentThreshold;
        [ShowOnly] public float AmountOverThreshold;

        private void Awake()
        {
            ResetValue();
        }

        /// <summary>
        /// Current threshold for the value.
        /// </summary>
        public float Threshold => currentThreshold;
        
        /// <summary>
        /// Returns true if the value is greater than the threshold
        /// </summary>
        public bool HasExceededThreshold => Value > Threshold;

        /// <summary>
        /// The current value
        /// </summary>
        public float Value => value;

        /// <summary>
        /// Increases the amount of the value by the give amount and checks if the amount has exceeded
        /// its threshold.
        /// </summary>
        /// <param name="deltaTime"></param>
        public void UpdateValue(float deltaTime)
        {
            value += deltaTime;
            AmountOverThreshold = Mathf.Max(0, Value - Threshold);
        }

        /// <summary>
        /// Resets the amount of the value to 0 and randomizes the threshold modifier.
        /// </summary>
        public void ResetValue()
        {
            value = 0;
            currentThreshold = baseThreshold + Random.Range(minThresholdModifier, maxThresholdModifier);
            AmountOverThreshold = 0;
        }
    }
}