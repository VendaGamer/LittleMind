using UnityEngine;

public class FPSController : PickController
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookUpAngle = 90f;
    [SerializeField] private float maxLookDownAngle = -90f;
    [SerializeField] private Transform playerCameraHolder;

    private Rigidbody rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody>();
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    protected override void Update()
    {
        base.Update();
        HandleLook();
        HandleMovement();
    }
    /// <summary>
    /// Stara se o otaceni kamery a hrace
    /// </summary>
    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Otaceni s hracem, samozrejmne jen horizontalne :D
        transform.Rotate(Vector3.up * mouseX);


        float newXRotation = playerCameraHolder.localEulerAngles.x - mouseY;

        // zajisteni ze nedevame obrovske stupne rotace
        if (newXRotation > 180f) newXRotation -= 360f;
        newXRotation = Mathf.Clamp(newXRotation, maxLookDownAngle, maxLookUpAngle);
        // Otaceni s kamerou
        playerCameraHolder.localEulerAngles = new Vector3(newXRotation, 0f, 0f);
    }
    /// <summary>
    /// Provadi pohyb hrace s pomoci unity input systemu
    /// </summary>
    private void HandleMovement()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = transform.right * moveHorizontal + transform.forward * moveVertical;

        rb.MovePosition(rb.position + moveSpeed * Time.deltaTime * movement.normalized);
    }
}