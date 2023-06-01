using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class creates singular location for other components to relate animation names to members of the
    /// InteractionType enum. These string names can then be accessed from anywhere in the game at runtime by providing
    /// an InteractionType.
    /// </summary>
    public class InteractionAnimations : MonoBehaviourSingleton<InteractionAnimations>
    {
        [SerializeField] private List<InteractionAnimationPair> interactionAnimationPairs = new List<InteractionAnimationPair>();

        private readonly Dictionary<InteractionType, string> animationNamesByInteractionType = new Dictionary<InteractionType, string>();
        
        public void Start()
        {
            //Translate the list to a dictionary for easier lookup and data validation
            for (int i = 0; i < interactionAnimationPairs.Count; i++)
            {
                InteractionAnimationPair pair = interactionAnimationPairs[i];
                
                if (!animationNamesByInteractionType.ContainsKey(pair.InteractionType))
                {
                    animationNamesByInteractionType.Add(pair.InteractionType, pair.animationTriggerName);
                    continue;
                }
                
                Debug.LogWarning("Multiple animation names associated with one interaction type, only the first will be accepted");
            }
        }

        
        /// <summary>
        /// Returns an animation name if one has been provided for the given interaction type. Returns null
        /// if none has been provided.
        /// </summary>
        /// <param name="interactionType"></param>
        /// <returns></returns>
        public string GetAnimationName(InteractionType interactionType)
        {
            foreach (InteractionAnimationPair interactionAnimationPair in interactionAnimationPairs)
            {
                if (interactionAnimationPair.InteractionType == interactionType)
                    return interactionAnimationPair.animationTriggerName;
            }

            return "";
        }
    }
}