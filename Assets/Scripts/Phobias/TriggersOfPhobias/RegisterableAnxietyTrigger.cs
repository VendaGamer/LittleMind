using UnityEngine;

public abstract class RegisterableAnxietyTrigger<T>:MonoBehaviour,IAnxietySource where T : AnxietyManager
{
    [SerializeField]
    protected AnxietyTriggerData Data;

    public float AnxietyAmount => Data.AnxietyAmount;
    public float InitialAnxietyAmount => Data.InitialAnxietyAmount;

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.TryGetComponent<T>(out var araPho)) 
            return;
        araPho.RegisterAnxietySource(this);
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.TryGetComponent<T>(out var araPho)) 
            return;
        araPho.UnRegisterAnxietySource(this);
    }
}