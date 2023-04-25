using UnityEngine;

public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance = null;
    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Debug.Log($"{this.GetType().Name} is being destroyed on {gameObject.name}, because a singelton reference was already set!", gameObject);
            Destroy(this);
            return;
        }

        _instance = this as T;
    }

    public void SetSingletonInstance()
    {
        _instance = this as T;
    }

    protected virtual void OnDestroy()
    {
        if(_instance == this)
        {
            _instance = null;
        }
    }
}
