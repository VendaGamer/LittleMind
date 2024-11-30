using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    [SerializeField] private Transform returnPoint;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.TryGetComponent<Nyctophobia>(out var nyctophobia))
        {
            nyctophobia.RecoverAnxiety();
        }
    }
}
