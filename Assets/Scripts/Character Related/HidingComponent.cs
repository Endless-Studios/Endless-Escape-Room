using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

/// <summary>
/// Character's Hiding Component handles the character's hidden state.
/// </summary>
public class HidingComponent : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private NavMeshObstacle navMeshObstacle;

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
        characterController.enabled = false;
        navMeshObstacle.enabled = false;
        OnHidingStarted.Invoke();
    }

    /// <summary>
    /// Character's public invoker for stopped hiding.
    /// </summary>
    public void StopHiding()
    {
        Hidden = false;
        characterController.enabled = true;
        navMeshObstacle.enabled = true;
        OnHidingEnded.Invoke();
    }
}
