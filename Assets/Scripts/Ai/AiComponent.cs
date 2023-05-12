using UnityEngine;

namespace Ai
{
    public abstract class AiComponent : MonoBehaviour
    {
        protected virtual void OnValidate()
        {
            facade ??= GetComponentInParent<AiFacade>();
        }

        protected AiFacade facade;
    }
}