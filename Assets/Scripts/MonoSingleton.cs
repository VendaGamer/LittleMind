using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;


public abstract class MonoSingleton<T> : MonoBehaviour, ISingleton where T : MonoSingleton<T>
{
    [CanBeNull]
    private static T _instance;
    [CanBeNull]
    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                Debug.LogError(typeof(T).ToString() + " is missing.");
            }

            return _instance;
        }
    }



    protected virtual void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
        }
        _instance = this as T;
    }


    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}