using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;


public abstract class MonoBehaviourSingleton<T> : MonoBehaviour, ISingleton where T : MonoBehaviourSingleton<T>
{
    [CanBeNull]
    private static T _instance;
    public static T Instance
    {
        get
        {
            Assert.IsNotNull(_instance, $"There is no instance of {typeof(T).Name}");

            return _instance;
        }
    }



    protected virtual void Awake()
    {
        if (_instance)
        {
            Destroy(_instance);
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