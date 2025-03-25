using UnityEngine;

public class BloodTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Hematophobia>(out var hemaPho))
            return;
        hemaPho.IncreaseAnxiety(0.05f);
    }
}
