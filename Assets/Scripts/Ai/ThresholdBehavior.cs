using UnityEngine;

namespace Ai
{
    public class ThresholdBehavior : MonoBehaviour
    {
        [SerializeField] private string thresholdName;
        [SerializeField] private float baseThreshold;
        [SerializeField] private float minThresholdModifier;
        [SerializeField] private float maxThresholdModifier;

        public float Threshold => baseThreshold + thresholdModifer;
        public bool HasExceededThreshold => Value > Threshold;
        public float AmountOverThreshold ;
        public float Value { get; private set; }
        
        private float thresholdModifer;

        public void UpdateValue(float deltaTime)
        {
            Value += deltaTime;
        }

        public void ResetValue()
        {
            Value = 0;
            thresholdModifer += Random.Range(minThresholdModifier, maxThresholdModifier);
        }
    }
}