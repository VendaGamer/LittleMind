using UnityEngine;

public class Cobweb : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent<ArachnoPhobia>(out var araPho)) 
            return;
        araPho.IncreaseAnxiety(0.05f);
    }
}
