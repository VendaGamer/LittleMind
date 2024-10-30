using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Nyctophobia>(out var nycPho))
            return;
        nycPho.
    }
}
