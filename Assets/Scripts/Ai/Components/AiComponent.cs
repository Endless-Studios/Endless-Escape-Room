using UnityEngine;

namespace Ai
{
    /// <summary>
    /// Base class for Ai Components that need a reference to either the AiEntity or AiReferences
    /// </summary>
    public abstract class AiComponent : MonoBehaviour
    {
        protected AiEntity entity;
        protected AiReferences references;
        protected AiAttributes attributes;
        protected GameplayInfo gameplayInfo;
        
        protected virtual void OnValidate()
        {
            if (!entity)
            {
                entity = GetComponentInParent<AiEntity>();
            }

            if (!references)
            {
                references = GetComponentInParent<AiReferences>();
            }

            if (!attributes)
            {
                attributes = GetComponentInParent<AiAttributes>();
            }

            if (!gameplayInfo)
            {
                gameplayInfo = GetComponentInParent<GameplayInfo>();
            }
        }
    }
}