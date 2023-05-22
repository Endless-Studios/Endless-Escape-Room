using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    public class PointOfInterest : MonoBehaviour
    {
        [SerializeField] private GameObject interactionPointObject;
        [SerializeField] private InteractionAnimationEnum interactionAnimation;

        public Vector3 InteractionPoint => interactionPointObject.transform.position;
        public Quaternion InteractionRotation => interactionPointObject.transform.rotation;

        public static readonly List<PointOfInterest> PointsOfInterest = new List<PointOfInterest>();

        protected virtual void Awake()
        {
            PointsOfInterest.Add(this);
        }

        protected virtual void OnDestroy()
        {
            PointsOfInterest.Remove(this);
        }
    }

    public enum InteractionAnimationEnum
    {
        This,
        That,
        TheOther
    }
}