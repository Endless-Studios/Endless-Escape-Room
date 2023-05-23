using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Puzzle for evaluating multiple Interaction Goals.
/// </summary>
public class InteractionPuzzle : Puzzle
{
    [SerializeField] private List<InteractionGoal> interactionGoals;

    private void Awake()
    {
        if (evaluateOnChange)
        {
            foreach (InteractionGoal interactionGoal in interactionGoals)
            {
                interactionGoal.OnCompleted.AddListener(SolutionChanged);
                interactionGoal.OnUncompleted.AddListener(SolutionChanged);
            }
        }
    }

    private void OnDestroy()
    {
        if (evaluateOnChange)
        {
            foreach (InteractionGoal interactionGoal in interactionGoals)
            {
                if (interactionGoal != null)
                {
                    interactionGoal.OnCompleted.RemoveListener(SolutionChanged);
                    interactionGoal.OnUncompleted.RemoveListener(SolutionChanged);
                }
            }
        }
    }

    internal override bool EvaluateSolutionInternal()
    {
        foreach (InteractionGoal interactionGoal in interactionGoals)
        {
            if (interactionGoal != null && interactionGoal.IsComplete == false)
            {
                return false;
            }
        }

        return true;
    }
}
