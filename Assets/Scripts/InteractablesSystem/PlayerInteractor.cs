using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] float interactDistance = 2;
    [SerializeField] bool debug = false;
    [SerializeField] LayerMask interactLayerMask;

    public UnityEvent<Interactable> OnItemInteracted = new UnityEvent<Interactable>();

    RaycastHit interactHit;

    Interactable lastHoveredInteractable = null;
    Grabbable currentGrabbable = null;

    // Update is called once per frame
    void Update()
    {
        if(currentGrabbable == null)
        {
            //If mouse cursor is active, raycast from mouse, otherwise from center of screen
            Ray interactRay;
            if (MouseLockHandler.Instance.IsMouseLocked)
                interactRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            else
                interactRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(debug)
                Debug.DrawLine(interactRay.origin, interactRay.origin + interactRay.direction * interactDistance, Color.blue);

            Interactable hoveredInteractable = null;
            if(playerInput.InteractEnabled && Physics.Raycast(interactRay, out interactHit, interactDistance, interactLayerMask))
            {
                hoveredInteractable = interactHit.collider.gameObject.GetComponentInParent<Interactable>();
                if(debug)
                    Debug.Log(interactHit.collider.gameObject.name);
                if(hoveredInteractable != null && hoveredInteractable.IsInteractable == false)
                    hoveredInteractable = null;
            }
            if(lastHoveredInteractable != hoveredInteractable)
            {
                if(debug)
                    Debug.Log($"Hovered changed: {lastHoveredInteractable?.gameObject.name} -> { hoveredInteractable?.gameObject.name}");
                if(lastHoveredInteractable != null)
                    lastHoveredInteractable.Unhighlight();
                lastHoveredInteractable = hoveredInteractable;
                if(hoveredInteractable)
                {
                    hoveredInteractable.Highlight();
                    PlayerHUD.Instance.SetInteractText(hoveredInteractable.InteractPrompt);

                }
                else
                    PlayerHUD.Instance.SetInteractText(string.Empty);
            }
            if(lastHoveredInteractable && playerInput.GetInteractPressed())
            {
                Interact(lastHoveredInteractable);
                currentGrabbable = lastHoveredInteractable as Grabbable;
            }
        }
        else if (playerInput.GetInteractReleased())
        {
            currentGrabbable.StopInteract();
            currentGrabbable = null;
        }
    }

    private void Interact(Interactable interactable)
    {
        interactable.Interact();
        OnItemInteracted.Invoke(interactable);
    }
}
