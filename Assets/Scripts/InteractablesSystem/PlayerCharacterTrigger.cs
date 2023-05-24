using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

///<summary>Events for Player Character Entering/Exiting the attached trigger.</summary>
public class PlayerCharacterTrigger : MonoBehaviour
{
    [SerializeField] private bool localPlayerOnly = true;

    public UnityEvent OnPlayerEnter = new UnityEvent();
    public UnityEvent OnPlayerExit = new UnityEvent();

    private void OnTriggerEnter(Collider collider)
    {
        PlayerCore playerCore = collider.GetComponent<PlayerCore>();

        if (playerCore && (!localPlayerOnly || PlayerCore.LocalPlayer == playerCore))
        {
            OnPlayerEnter.Invoke();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        PlayerCore playerCore = collider.GetComponent<PlayerCore>();

        if (playerCore && (!localPlayerOnly || PlayerCore.LocalPlayer == playerCore))
        {
            OnPlayerExit.Invoke();
        }
    }
}
