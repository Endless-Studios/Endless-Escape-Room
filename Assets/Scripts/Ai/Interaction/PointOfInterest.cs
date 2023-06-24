using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ai
{
    /// <summary>
    /// This class provides information to the Ai about the various things it can interact with in the scene,
    /// it also provides the positions, rotations, and kind of interactions necessary to do so.
    /// </summary>
    public class PointOfInterest : MonoBehaviour, IAiInteractionTypeSource
    {
        /// <summary>
        /// A static list of points of interest that each point of interest adds to and removes itself from.
        /// </summary>
        public static readonly List<PointOfInterest> PointsOfInterest = new List<PointOfInterest>();
        
        [SerializeField] private GameObject interactionPointObject;
        [SerializeField] private Collider lineOfSightCollider;
        [SerializeField] private InteractionType interactionType;

        public UnityEvent OnInteracted = new UnityEvent();

        public InteractionType InteractionType => interactionType;
        public Vector3 InteractionPoint => interactionPointObject.transform.position;
        public Quaternion InteractionRotation => interactionPointObject.transform.rotation;
        public Collider LineOfSightCollider => lineOfSightCollider;
        
        [ContextMenu("Interact")]
        public void Interact()
        {
            OnInteracted?.Invoke();
        }
        
        protected virtual void OnEnable()
        {
            PointsOfInterest.Add(this);
        }

        protected virtual void OnDisable()
        {
            PointsOfInterest.Remove(this);
        }
    }
}