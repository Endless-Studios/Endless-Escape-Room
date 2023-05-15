using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void EvaluateSolution()
    {
        foreach (InteractionGoal interactionGoal in interactionGoals)
        {
            if (interactionGoal.IsComplete == false)
            {
                IsComplete = false;
                return;
            }
        }

        IsComplete = true;
    }
}
