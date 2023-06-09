using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    /// <summary>
    /// This class creates singular location for other components access animation names.
    /// </summary>
    public class AnimationNames : MonoBehaviourSingleton<AnimationNames>
    {
        [SerializeField] private List<InteractionAnimationPair> interactionAnimationPairs = new List<InteractionAnimationPair>();
        [SerializeField] private List<string> monsterFidgetNames = new List<string>();

        private readonly Dictionary<InteractionType, string> animationNamesByInteractionType = new Dictionary<InteractionType, string>();
        private readonly List<string> monsterFidgetNamesList = new List<string>();
        private readonly Dictionary<AiType, List<string>> fidgetNameListByAiType = new Dictionary<AiType, List<string>>();

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

            foreach (string fidgetName in monsterFidgetNames)
            {
                if (monsterFidgetNamesList.Contains(fidgetName))
                {
                    Debug.LogWarning("Duplicate animation name in the monsterFidgetNames list, duplicates will be ignored");
                }
                monsterFidgetNamesList.Add(fidgetName);
            }
            
            fidgetNameListByAiType.Add(AiType.Monster, monsterFidgetNamesList);
        }

        
        /// <summary>
        /// Returns an animation name if one has been provided for the given interaction type. Returns null
        /// if none has been provided.
        /// </summary>
        /// <param name="interactionType"></param>
        /// <returns></returns>
        public string GetInteractionAnimationName(InteractionType interactionType)
        {
            foreach (InteractionAnimationPair interactionAnimationPair in interactionAnimationPairs)
            {
                if (interactionAnimationPair.InteractionType == interactionType)
                    return interactionAnimationPair.animationTriggerName;
            }

            return "";
        }

        public string GetRandomFidgetAnimation(AiType aiType)
        {
            if (fidgetNameListByAiType.TryGetValue(aiType, out List<string> fidgetNames) && fidgetNames.Count > 0)
            {
                int index = Random.Range(0, fidgetNames.Count);
                return fidgetNames[index];
            }
            
            Debug.LogWarning("No fidget names found for the provided AiType");
            return "";
        }
        
    }

    public enum AiType
    {
        Monster,
        Zombie,
        Ghost
    }
}