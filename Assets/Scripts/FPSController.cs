using UnityEngine;

public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookUpAngle = 90f;
    [SerializeField] private float maxLookDownAngle = -90f;

    private Camera playerCamera;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCamera = GetComponentInChildren<Camera>();
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate the camera vertically
        Vector3 currentRotation = playerCamera.transform.localEulerAngles;
        float newXRotation = currentRotation.x - mouseY;

        // Clamp the vertical rotation
        if (newXRotation > 180f) newXRotation -= 360f;
        newXRotation = Mathf.Clamp(newXRotation, maxLookDownAngle, maxLookUpAngle);

        playerCamera.transform.localEulerAngles = new Vector3(newXRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;

        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
    }
}