using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ai
{
    /// <summary>
    /// This class provides information to the Ai about the various things it can interact with in the scene,
    /// it also provides the positions, rotations, and kind of interactions necessary to do so.
    /// </summary>
    public class PointOfInterest : MonoBehaviour
    {
        /// <summary>
        /// A static list of points of interest that each point of interest adds to and removes itself from.
        /// </summary>
        public static readonly List<PointOfInterest> PointsOfInterest = new List<PointOfInterest>();
        
        [SerializeField] private GameObject interactionPointObject;
        [SerializeField] private Collider lineOfSightCollider;

        public UnityEvent<AiEntity> OnAiInteracted = new UnityEvent<AiEntity>();
        public UnityEvent OnInteractionInterrupted = new UnityEvent();
        
        public Vector3 InteractionPoint => interactionPointObject.transform.position;
        public Quaternion InteractionRotation => interactionPointObject.transform.rotation;
        public Collider LineOfSightCollider => lineOfSightCollider;
        
        [ContextMenu("Interact")]
        public void AiInteract(AiEntity aiEntity)
        {
            OnAiInteracted?.Invoke(aiEntity);
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