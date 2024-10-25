using UnityEngine;

public class ArachnoPhobia : MentalIllness
{
    [SerializeField] private LayerMask spiderLayer;

    private void Start()
    {
        // Dame hraci vsechny symptomy
        RequireSymptom<VisualDistortion>();
        RequireSymptom<Trembling>();
    }
    protected override void FixedUpdate()
    {
        CheckForNearbySpiders();
        base.FixedUpdate();
    }
    private void CheckForNearbySpiders()
    {
        Vector3 currentPosition = transform.position;
        var colliders = Physics.OverlapSphere(currentPosition, triggerDistance, spiderLayer);

        if (colliders == null || colliders.Length == 0) return;

        Collider closestCollider = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            float dSqrToCollider = (collider.transform.position - currentPosition).sqrMagnitude;
            if (dSqrToCollider < closestDistanceSqr)
            {
                closestCollider = collider;
                closestDistanceSqr = dSqrToCollider;
            }
        }

        float distance = Vector3.Distance(currentPosition, closestCollider.transform.position);
        float intensity = 1f - (distance / triggerDistance);
        PendNewAnxietyLevel(intensity);
    }

    private void OnTriggerStay(Collider other)
    {
        //zjisteni zda je objekt v vrstve spider (pavucinky)
        if (spiderLayer.value == other.gameObject.layer)
        {
            PendNewAnxietyLevel(1f);
        }
    }
}