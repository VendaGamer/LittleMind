using UnityEngine;

public class PickController : MonoBehaviour
{
    private GameObject holding;
    private Camera playerCamera;
    [SerializeField]
    private float rayDistance = 100f;

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }
    private void PickUpObject(GameObject gameObject)
    {
        holding = gameObject;
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
                    PickUpObject(gameObject);
                }
            }
        }
    }
}
