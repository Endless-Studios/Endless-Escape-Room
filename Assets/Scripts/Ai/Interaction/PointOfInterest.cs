using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Ai
{
    /// <summary>
    /// This class provides information to the Ai about the various things it can interact with in the scene,
    /// it also provides the positions, rotations, and kind of interactions necessary to do so.
    /// </summary>
    [SelectionBase]
    public class PointOfInterest : MonoBehaviour
    {
        /// <summary>
        /// A static list of points of interest that each point of interest adds to and removes itself from.
        /// </summary>
        public static readonly List<PointOfInterest> PointsOfInterest = new List<PointOfInterest>();
        
        [Tooltip("The AI will stand at this position/rotation to interact with this object")]
        [SerializeField] private GameObject aiInteractionTransform;
        [SerializeField] private InteractionType interactionType;
        [SerializeField] bool finishInteractionAutomatically = true;

        public UnityEvent<AiEntity> OnAiInteracted = new UnityEvent<AiEntity>();
        public UnityEvent<AiEntity> OnAiInteractionAnimation = new UnityEvent<AiEntity>();
        public UnityEvent OnInteractionInterrupted = new UnityEvent();
        
        public Vector3 InteractionPoint => aiInteractionTransform.transform.position;
        public Quaternion InteractionRotation => aiInteractionTransform.transform.rotation;

        /// <summary>
        /// Invokes the OnAiInteracted event
        /// </summary>
        /// <param name="aiEntity"></param>
        internal void AiInteract(AiEntity aiEntity)
        {
            aiEntity.References.AnimationComponent.PlayInteractionAnimation(interactionType);
            OnAiInteracted.Invoke(aiEntity);
            if(interactionType == InteractionType.None && finishInteractionAutomatically)
                aiEntity.FinishedInteraction();
        }

        internal void AiAnimationInteractionEvent(AiEntity aiEntity)
        {
            if(finishInteractionAutomatically)
                aiEntity.FinishedInteraction();
            OnAiInteractionAnimation.Invoke(aiEntity);
        }

        /// <summary>
        /// Invokes the OnInteractionInterrupted event
        /// </summary>
        public void InteractionInterrupted()
        {
            OnInteractionInterrupted.Invoke();
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