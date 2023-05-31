using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Base class representing a puzzle in the game.
/// </summary>
[SelectionBase]
public abstract class Puzzle : MonoBehaviour
{
    public UnityEvent OnCompleted = new UnityEvent();
    public UnityEvent OnUncompleted = new UnityEvent();
    [Tooltip("Evaluate solution failed. true: manual evaluation | false: automatic evaluation (evaluate on change)")]
    public UnityEvent<bool> OnFailedSolution = new UnityEvent<bool>();

    [SerializeField] private bool allowUncompleting = false;
    [SerializeField] protected bool evaluateOnChange = true;

    private bool complete;
    public bool IsComplete
    {
        get
        {
            return complete;
        }
        private set
        {
            if (complete && allowUncompleting == false)
                return;

            if (complete != value)
            {
                complete = value;

                if (value == true)
                    OnCompleted.Invoke();
                else
                    OnUncompleted.Invoke();
            }
        }
    }

    /// <summary>
    /// Checks all puzzle conditions, applies IsComplete, and Invokes completed events based on result.
    /// </summary>
    public void EvaluateSolution()
    {
        bool result = EvaluateSolutionInternal();

        if (result == false)
        {
            OnFailedSolution.Invoke(true);
        }

        IsComplete = result;
    }

    /// <summary>
    /// Internal method to evaluate the puzzle solution.
    /// </summary>
    /// <returns>True if the puzzle is solved.</returns>
    internal abstract bool EvaluateSolutionInternal();

    /// <summary>
    /// Called when the puzzle solution is changed.
    /// </summary>
    public void SolutionChanged()
    {
        if (evaluateOnChange == false)
            return;

        bool result = EvaluateSolutionInternal();

        if (result == false)
        {
            OnFailedSolution.Invoke(false);
        }

        IsComplete = result;
    }
}
