using UnityEngine;
using UnityEngine.Events;

public abstract class InteractionGoal : MonoBehaviour
{
    public UnityEvent OnCompleted = new UnityEvent();
    public UnityEvent OnUncompleted = new UnityEvent();

    private bool complete;
    public bool IsComplete
    {
        get { return complete; }

        set
        {
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
}