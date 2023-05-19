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
        foreach (InteractionGoal interactionGoal in interactionGoals)
        {
            interactionGoal.OnCompleted.AddListener(SolutionChanged);
            interactionGoal.OnUncompleted.AddListener(SolutionChanged);
        }
    }

    private void OnDestroy()
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
