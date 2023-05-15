using UnityEngine;

namespace Ai
{
    public abstract class AiComponent : MonoBehaviour
    {
        protected virtual void OnValidate()
        {
            Entity ??= GetComponentInParent<AiEntity>();
            references ??= GetComponentInParent<AiReferences>();
        }

        protected AiEntity Entity;
        protected AiReferences references;
    }
}