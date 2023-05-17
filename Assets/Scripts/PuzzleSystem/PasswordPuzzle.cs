using UnityEngine;
using UnityEngine.Events;

public class PasswordPuzzle : Puzzle
{
    public UnityEvent<string> OnInputStringUpdated;
    [SerializeField] private string solutionString;

    [Tooltip("Clamp input character count. (0 is unclamped)")]
    [SerializeField, Min(0)] private int inputClamp = 0;
    [SerializeField] private bool evaluateOnChange = false;

    public string SolutionString => solutionString;

    private string currentInputString;
    public string CurrentInputString
    {
        get
        {
            return currentInputString;
        }        
        protected set
        {
            if (value.Equals(CurrentInputString))
                return;

            currentInputString = value;
            OnInputStringUpdated.Invoke(value);            

            if (evaluateOnChange)
                EvaluateSolution();
        }
    }

    private void Awake()
    {
        if (inputClamp > 0 && solutionString.Length > inputClamp)
        {
            Debug.LogWarning("Solution string is longer than input clamp. Clamping solution.");
            solutionString = solutionString.Substring(0, inputClamp);
        }            
    }

    internal override bool EvaluateSolutionInternal()
    {
        return CurrentInputString.Equals(solutionString, System.StringComparison.OrdinalIgnoreCase);
    }

    ///<summary>Replaces current input cache with the given string.</summary>
    ///<parm name="newInputString">The string to apply as new input.</param>
    public void OverwriteCurrentInput(string newInputString)
    {
        if (inputClamp > 0 && newInputString.Length > inputClamp)
            newInputString = newInputString.Substring(0, inputClamp);

        CurrentInputString = newInputString;
    }

    ///<summary>Adds a string to the end of the current input cache.</summary>
    ///<parm name="addString">The string to add to the input cache.</param>
    public void AddToCurrentInput(string addString)
    {
        OverwriteCurrentInput(CurrentInputString + addString);
    }

    ///<summary>Remove a single character from the current input cache.</summary>
    public void RemoveSingleCharacter()
    {
        if (CurrentInputString.Length > 0)
            CurrentInputString = CurrentInputString.Remove(CurrentInputString.Length - 1, 1);
    }

    ///<summary>Clears the current input cache.</summary>
    public void ClearCurrentInput()
    {
        CurrentInputString = string.Empty;
    }
}
