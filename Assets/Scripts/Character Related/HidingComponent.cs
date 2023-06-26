using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Character's Hiding Component handles the character's hidden state.
/// </summary>
public class HidingComponent : MonoBehaviour
{
    /// <summary>
    /// Character started hiding event. 
    /// </summary>
    public UnityEvent OnHidingStarted;

    /// <summary>
    /// Character ended hiding event. 
    /// </summary>
    public UnityEvent OnHidingEnded;

    public bool Hidden { get; private set; } = false;

    /// <summary>
    /// Character's public invoker for started hiding. 
    /// </summary>
    public void StartHiding()
    {
        Hidden = true;
        PlayerCore.LocalPlayer.PlayerTarget.enabled = false;
        OnHidingStarted.Invoke();
    }

    /// <summary>
    /// Character's public invoker for stopped hiding.
    /// </summary>
    public void StopHiding()
    {
        Hidden = false;
        PlayerCore.LocalPlayer.PlayerTarget.enabled = true;
        OnHidingEnded.Invoke();
    }
}
