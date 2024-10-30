using System.Collections;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    [SerializeField]
    private string displayName;
    [SerializeField]
    private string description;
    public string DisplayName => displayName;
    public string Description => description;
    private Color originalColor;
    private Rigidbody rb;
    private Collider col;
    private Renderer ren;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponentInChildren<Collider>();
        ren = GetComponentInChildren<Renderer>();
        originalColor = ren.material.color;
    }
    public void DropObject()
    {
        StopAllCoroutines();
        rb.isKinematic = false;
        col.isTrigger = false;
        ren.material.color = originalColor;
        transform.SetParent(null);
    }
    public void PickObject(Transform parent,float pickDur)
    {
        rb.isKinematic = true;
        col.isTrigger = true;
        StartCoroutine(PerformPickupLerp(parent, pickDur));
    }

    private IEnumerator PerformPickupLerp(Transform pickupPoint, float pickDur)
    {
        transform.SetParent(pickupPoint);
        var endPos = Vector3.zero;
        var endRot = Quaternion.Euler(Vector3.zero);

        transform.GetLocalPositionAndRotation(out Vector3 startPos, out Quaternion startRot);
        float elapsedTime = 0f;
        while (elapsedTime < pickDur)
        {
            elapsedTime += Time.deltaTime;
            var step = Mathf.SmoothStep(0, 1, elapsedTime / pickDur);

            transform.SetLocalPositionAndRotation(
                Vector3.Lerp(startPos, endPos, step),
                Quaternion.Lerp(startRot, endRot, step)
            );
            yield return null;
        }
        ren.material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.70f);
    }
}
