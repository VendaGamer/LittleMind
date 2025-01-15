using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    [SerializeField] private Transform returnPoint;
    /// <summary>
    /// Bod ke kteremu hrac dojde pokud ztrati kontrolu a bude tato louce nejbliz
    /// </summary>
    public Transform ReturnPoint => returnPoint;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Nyctophobia>(out var nyctophobia))
        {
            nyctophobia.RecoverAnxiety();
        }
    }
}
