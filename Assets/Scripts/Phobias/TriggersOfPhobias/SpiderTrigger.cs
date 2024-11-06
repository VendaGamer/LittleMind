using UnityEngine;

public class SpiderTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.TryGetComponent<ArachnoPhobia>(out var araPho))
            return;
        var distance = Vector3.Distance(transform.position, other.transform.position);
        araPho.PendNewAnxietyLevelBasedOnDistance(distance);
    }
}
