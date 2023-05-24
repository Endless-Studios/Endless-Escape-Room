using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component for allowing Rigidbodies and characters to land on and move with moving transforms. Uses attached Trigger enter/exit.
/// </summary>
public class MovableSurface : MonoBehaviour
{
    private void OnTriggerEnter(Collider enteredCollider)
    {
        if (enteredCollider.attachedRigidbody != null)
        {
            enteredCollider.attachedRigidbody.transform.SetParent(transform);
        }
        else
        {
            CharacterMovement characterMovement = enteredCollider.GetComponent<CharacterMovement>();

            if (characterMovement != null)
            {
                characterMovement.SetMovingSurface(transform);
            }
        }
    }

    private void OnTriggerExit(Collider exitedCollider)
    {
        if (exitedCollider.attachedRigidbody != null)
        {
            exitedCollider.attachedRigidbody.transform.SetParent(null);
        }
        else
        {
            CharacterMovement characterMovement = exitedCollider.GetComponent<CharacterMovement>();

            if (characterMovement != null)
            {
                characterMovement.UnsetMovingSurface(transform);
            }
        }
    }
}
