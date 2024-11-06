using UnityEngine;

public class HighObjectTrigger : MonoBehaviour
{
    private void OnCollisionStay(Collision other)
    {
        if (!other.gameObject.TryGetComponent<Nyctophobia>(out var nycPho))
            return;
        
    }
}
