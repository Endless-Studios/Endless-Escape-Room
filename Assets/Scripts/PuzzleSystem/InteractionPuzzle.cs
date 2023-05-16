using System.Collections.Generic;
using UnityEngine;

/// <summary>Puzzle for evaluating multiple Interaction Goals.</summary>
public class InteractionPuzzle : Puzzle
{
    [SerializeField] private List<InteractionGoal> interactionGoals;

    private void Awake()
    {
        foreach (InteractionGoal interactionGoal in interactionGoals)
        {
            interactionGoal.OnCompleted.AddListener(EvaluateSolution);
            interactionGoal.OnUncompleted.AddListener(EvaluateSolution);
        }
    }

    internal override bool EvaluateSolutionInternal()
    {
        foreach (InteractionGoal interactionGoal in interactionGoals)
        {
            if (interactionGoal.IsComplete == false)
            {
                return false;
            }
        }

        return true;
    }
}
