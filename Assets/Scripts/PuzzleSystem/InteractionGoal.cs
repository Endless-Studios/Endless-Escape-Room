using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Interaction Goal base class. Goals can fire off individual events, or be used in an Interaction Puzzle to combine multiple goals.
/// </summary>
public abstract class InteractionGoal : MonoBehaviour
{
    public UnityEvent OnCompleted = new UnityEvent();
    public UnityEvent OnUncompleted = new UnityEvent();

    [SerializeField] private bool isUncompletable = true;

    private bool complete;
    public bool IsComplete
    {
        get
        {
            return complete;
        }
        set
        {
            if (complete != value && (isUncompletable || complete == false))
            {
                complete = value;

                if (value == true)
                    OnCompleted.Invoke();
                else
                    OnUncompleted.Invoke();
            }
        }
    }
}