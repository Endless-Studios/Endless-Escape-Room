using UnityEngine;
using UnityEngine.Events;

public abstract class Puzzle : MonoBehaviour
{
    public UnityEvent OnCompleted = new UnityEvent();
    public UnityEvent OnUncompleted = new UnityEvent();
    [SerializeField] private bool puzzleStaysCompleted = true;

    private bool complete;
    public bool IsComplete
    {
        get { return complete; }

        set
        {

            if (complete && puzzleStaysCompleted)
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

    /// <summary>Checks all puzzle conditions, applies IsComplete, and Invokes completed events based on result.</summary>
    public void EvaluateSolution()
    {
        IsComplete = EvaluateSolutionInternal();
    }

    internal abstract bool EvaluateSolutionInternal();
}
