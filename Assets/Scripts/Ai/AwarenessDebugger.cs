using System;
using UnityEngine;

namespace Ai
{
    internal class AwarenessDebugger : MonoBehaviour
    {
        [SerializeField] private AwarenessComponent awarenessComponent;

        private void Awake()
        {
            awarenessComponent.GainedNewStimulus.AddListener(HandleGainedNewStimulus);
        }

        private void HandleGainedNewStimulus()
        {
            Vector3 position = awarenessComponent.CurrentStimulus.Position;
            Color color = awarenessComponent.CurrentStimulus.SenseKind switch
            {
                SenseKind.Sight => Color.blue,
                SenseKind.Hearing => Color.red,
                SenseKind.Proximity => Color.yellow,
                SenseKind.Undefined => Color.cyan,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            Debug.DrawLine(position, position + Vector3.up, color, 5f, false);
        }
    }
}