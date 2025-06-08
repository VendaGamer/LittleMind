using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

[DefaultExecutionOrder(-70)]
public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
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