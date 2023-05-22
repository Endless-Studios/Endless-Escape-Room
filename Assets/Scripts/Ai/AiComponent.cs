using UnityEngine;

namespace Ai
{
    /// <summary>
    /// Base class for Ai Components that need a reference to either the AiEntity or AiReferences
    /// </summary>
    public abstract class AiComponent : MonoBehaviour
    {
        protected AiEntity Entity;
        protected AiReferences references;
        
        protected virtual void OnValidate()
        {
            if (!Entity)
            {
                Entity = GetComponentInParent<AiEntity>();
            }

            if (!references)
            {
                references = GetComponentInParent<AiReferences>();
            }
        }
    }
}