using UnityEngine;
using System.Linq;

/// <summary>
/// Interaction Goal for a Snappable evaluating the snapped Pickupable's identifiers.
/// </summary>
public class SnappableGoal : InteractionGoal
{
    [SerializeField] private Snappable snappable;
    [SerializeField] Identifier[] goalSuccessIdentifiers;
    [SerializeField] RequirementType requirementType = RequirementType.Any;

    private void OnValidate()
    {
        if (snappable == null)
            snappable = GetComponent<Snappable>();
    }

    private void Awake()
    {
        if (snappable != null)
        {
            snappable.OnObjectSnapped.AddListener(HandleObjectSnapped);
            snappable.OnObjectRemoved.AddListener(HandleObjectRemoved);
        }            
    }

    private void OnDestroy()
    {
        if (snappable != null)
        {
            snappable.OnObjectSnapped.RemoveListener(HandleObjectSnapped);
            snappable.OnObjectRemoved.RemoveListener(HandleObjectRemoved);
        }            
    }

    private void HandleObjectSnapped(Pickupable pickupable)
    {
        if(PickupableMeetsGoal(pickupable))
        {
            IsComplete = true;
        }       
    }

    private void HandleObjectRemoved()
    {
        IsComplete = false;
    }

    private bool PickupableMeetsGoal(Pickupable pickupable)
    {
        switch (requirementType)
        {
            case RequirementType.Any:
                if (goalSuccessIdentifiers.Length == 0)
                    return true;
                foreach (Identifier identifier in goalSuccessIdentifiers)
                {
                    if (pickupable.Identifiers.Contains(identifier))
                        return true;
                }
                return false;
            case RequirementType.All:
                foreach (Identifier identifier in goalSuccessIdentifiers)
                {
                    if (pickupable.Identifiers.Contains(identifier) == false)
                        return false;
                }
                return true;
            default:
                return false;
        }
    }
}
