using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] ItemInspector itemInspector;
    [SerializeField] float interactDistance = 2;
    [SerializeField] bool debug = false;
    [SerializeField] LayerMask interactLayerMask;

    public UnityEvent<Interactable> OnItemInteracted = new UnityEvent<Interactable>();

    public bool HasTarget => lastHoveredInteractable != null;

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

                if(hoveredInteractable != null && itemInspector.IsInspecting)
                {
                    //Determine if this item has our held item as a parent, if not then its not valid to interact with right now!
                    Transform currentTransform = hoveredInteractable.transform;
                    while(currentTransform != itemInspector.CurrentInspectable.transform && currentTransform.parent != null)
                        currentTransform = currentTransform.parent;
                    if(currentTransform != itemInspector.CurrentInspectable.transform)
                        hoveredInteractable = null;
                }

                if(hoveredInteractable != null && hoveredInteractable.IsInteractable == false)
                    hoveredInteractable = null;
            }
            if(lastHoveredInteractable != hoveredInteractable)
            {
                ItemLayer itemLayer;
                if(itemInspector.IsInspecting)
                    itemLayer = ItemLayer.Held;
                else
                    itemLayer = ItemLayer.World;

                if(debug)
                    Debug.Log($"Hovered changed: {lastHoveredInteractable?.gameObject.name} -> { hoveredInteractable?.gameObject.name}");
                if(lastHoveredInteractable != null)
                    lastHoveredInteractable.Unhighlight(itemLayer);
                lastHoveredInteractable = hoveredInteractable;
                if(hoveredInteractable)
                {
                    hoveredInteractable.Highlight(itemLayer);
                    PlayerHUD.Instance.SetInteractText(hoveredInteractable.InteractPrompt);
                }
                else
                    PlayerHUD.Instance.SetInteractText(string.Empty);
            }
            if(lastHoveredInteractable && playerInput.GetInteractPressed())
            {
                Interact(lastHoveredInteractable);
                currentGrabbable = lastHoveredInteractable as Grabbable;

                //Just in case interacting changed the text - a common case.
                PlayerHUD.Instance.SetInteractText(hoveredInteractable.InteractPrompt);
            }
        }
        else if (playerInput.GetInteractReleased())
        {
            currentGrabbable.StopInteract();
            currentGrabbable = null;
        }
        else
        {
            currentGrabbable.HandleUpdate();
        }
    }

    private void Interact(Interactable interactable)
    {
        interactable.Interact();
        OnItemInteracted.Invoke(interactable);
    }
}
