using UnityEngine;

public class BloodTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent<Hematophobia>(out var hemaPho))
            return;
        hemaPho.PendNewAnxietyLevel(1.5f);
    }
}
