using UnityEngine;

namespace Ai
{
    public class ThresholdBehavior : MonoBehaviour
    {
        [SerializeField] private string thresholdName;
        [SerializeField] private float baseThreshold;
        [SerializeField] private float minThresholdModifier;
        [SerializeField] private float maxThresholdModifier;
        [ShowOnly] public float AmountOverThreshold;
        
        public float Threshold => baseThreshold + thresholdModifer;
        public bool HasExceededThreshold => Value > Threshold;
        public float Value { get; private set; }
        
        private float thresholdModifer;

        public void UpdateValue(float deltaTime)
        {
            Value += deltaTime;
            AmountOverThreshold = Mathf.Max(0, Value - Threshold);
        }

        public void ResetValue()
        {
            Value = 0;
            thresholdModifer += Random.Range(minThresholdModifier, maxThresholdModifier);
            AmountOverThreshold = 0;
        }
    }
}