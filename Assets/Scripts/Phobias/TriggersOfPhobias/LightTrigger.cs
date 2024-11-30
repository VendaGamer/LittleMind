using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    [SerializeField] private Transform returnPoint;
    /// <summary>
    /// Bod ke kteremu hrac dojde
    /// </summary>
    public Transform ReturnPoint => returnPoint;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Nyctophobia>(out var nyctophobia))
        {
            nyctophobia.RecoverAnxiety();
        }
    }
}
