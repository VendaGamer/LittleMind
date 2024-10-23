using UnityEngine;

public abstract class PickController : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField]
    private Transform pickupPoint;
    private PickableObject holding;
    [SerializeField]
    protected Camera playerCamera { get; private set;}
    [SerializeField]
    private float rayDistance = 3f;

    protected virtual void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }
    private void PickUpObject(PickableObject pickableObject)
    {
        holding = pickableObject;
        pickableObject.transform.SetParent(pickupPoint);
        pickableObject.transform.localPosition = Vector3.zero;
        pickableObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
        pickableObject.transform.localScale = Vector3.one;

        if (pickableObject.gameObject.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }
        if (pickableObject.gameObject.TryGetComponent<Collider>(out var col))
        {
            col.isTrigger = true;
        }
    }
    private void DropObject()
    {
        holding.transform.SetParent(null);
        if (holding.gameObject.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
        }
        if (holding.gameObject.TryGetComponent<Collider>(out var col))
        {
            col.isTrigger = false;
        }
        holding = null;
    }
    protected virtual void Update()
    {
        if (holding == null) //pickup logic
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
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
        else //drop logic
        {
            if (Input.GetButton("Drop"))
            {
                DropObject();
            }
        }
    }
}
