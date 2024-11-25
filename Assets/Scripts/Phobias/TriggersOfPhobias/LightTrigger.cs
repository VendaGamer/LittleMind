using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Nyctophobia>(out var nyctophobia))
        {
            nyctophobia.RecoverAnxiety();
        }
    }
}
