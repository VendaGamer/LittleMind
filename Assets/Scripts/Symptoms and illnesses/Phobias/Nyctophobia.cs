using UnityEngine;
public class Nyctophobia : MentalIllness
{
    [SerializeField] private LayerMask savePointLayer;
    [SerializeField] private LayerMask lightLayer;
    [SerializeField] private float LightDistanceTrigger = 6f;
    private void Start()
    {
        RequireSymptom<Trembling>();
        RequireSymptom<VisualDistortion>();
        RequireSymptom<HeartBeat>();
    }
    private void CheckForLight()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, LightDistanceTrigger, lightLayer);
        foreach (Collider collider in colliders) 
        {
            collider.TryGetComponent<Light>(out Light light);
            if(light != null)
            {

                return;
            }
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(savePointLayer.value == other.gameObject.layer)
        {
            enabled = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (savePointLayer.value == other.gameObject.layer)
        {
            enabled = true;
        }
    }
    private void OnDisable()
    {
        RecoverFromSymptoms();
    }
}
