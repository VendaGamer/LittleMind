using UnityEngine;

public class PickController : MonoBehaviour
{
    [SerializeField]
    private Transform pickupPoint;
    private PickableObject holding;
    private Camera playerCamera;
    [SerializeField]
    private float rayDistance = 100f;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }
    private void PickUpObject(PickableObject gameObject)
    {
        if (holding != null) return;
        holding = gameObject;
        gameObject.transform.position = pickupPoint.position;
        var rb = gameObject.gameObject.GetComponent<Rigidbody>();
        rb.
        gameObject.transform.parent = pickupPoint;
    }
    void Update()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            var gameObject = hit.collider.gameObject;
            if (gameObject == null) return;
            if (gameObject.TryGetComponent<PickableObject>(out var pickableObject))
            {
                Debug.Log("Can pickup gameobject named: " + pickableObject.DisplayName);
                if (Input.GetButton("Pickup"))
                {
                    PickUpObject(pickableObject);
                }
            }
        }
    }
}
