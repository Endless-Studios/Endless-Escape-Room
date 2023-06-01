using System;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// For debugging use only.
    /// </summary>
    internal class AwarenessDebugger : AiComponent
    {
        [SerializeField] private AwarenessComponent awarenessComponent;

        private void Awake()
        {
            awarenessComponent.GainedNewStimulus.AddListener(HandleGainedNewStimulus);
        }

        private void HandleGainedNewStimulus()
        {
            Vector3 position = gameplayInfo.CurrentStimulus.Position;
            Color color;
            if (gameplayInfo.CurrentStimulus.SenseKind == SenseKind.Sight)
                color = Color.blue;
            else if (gameplayInfo.CurrentStimulus.SenseKind == SenseKind.Hearing)
                color = Color.red;
            else if (gameplayInfo.CurrentStimulus.SenseKind == SenseKind.Proximity)
                color = Color.yellow;
            else if (gameplayInfo.CurrentStimulus.SenseKind == SenseKind.Undefined)
                color = Color.cyan;
            else
                throw new ArgumentOutOfRangeException();

            Debug.DrawLine(position, position + Vector3.up, color, 5f, false);
        }
    }
}