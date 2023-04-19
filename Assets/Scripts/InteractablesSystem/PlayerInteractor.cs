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

    RaycastHit[] interactHits = new RaycastHit[1];

    Interactable lastHoveredInteractable = null;

    // Update is called once per frame
    void Update()
    {
        Ray interactRay = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if(debug)
            Debug.DrawLine(interactRay.origin, interactRay.origin + interactRay.direction * interactDistance, Color.blue);

        Interactable hoveredInteractable = null;
        int hits = Physics.RaycastNonAlloc(interactRay, interactHits, interactDistance, interactLayerMask);
        if(hits > 0)
        {
            hoveredInteractable = interactHits[0].collider.gameObject.GetComponent<Interactable>();
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
                hoveredInteractable.Highlight();
        }
        if (lastHoveredInteractable && playerInput.GetInteractPressed())
        {
            Interact(lastHoveredInteractable);
        }
    }

    private void Interact(Interactable interactable)
    {
        interactable.Interact();
        OnItemInteracted.Invoke(interactable);
    }
}
