using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviour, IInteractor
{
    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float sprintSpeed = 7f;

    [SerializeField]
    private float jumpForce = 2f;

    [SerializeField]
    private Transform jumpPoint;

    [SerializeField]
    private float maxJumpPointDist = 0.001f;

    [SerializeField]
    private float jumpCooldown = 0.1f;

    [SerializeField]
    private float jumpPointRadius = 0.3f;

    [SerializeField]
    private LayerMask groundLayerMask;
    private bool canJump = true;
    private bool isGrounded;
    private bool isCrouching;
    private static Camera playerCamera => PlayerCamera.Instance.Camera;

    [Header("Look Settings")]
    [SerializeField]
    private float maxLookUpAngle = 90f;

    [SerializeField]
    private float maxLookDownAngle = -90f;

    [SerializeField]
    private Transform playerCameraHolder;

    public static Controls Controls { get; private set; }
    private float currentSpeed;
    private Rigidbody rb;
    private bool isRunning;
    private Animator animator;

    private void Awake()
    {
        Controls = new Controls();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = moveSpeed;
        SwitchGlobalInteractions(globalInteractionsPlayerControls);
    }

    private void OnDisable()
    {
        Controls.Player.Disable();
        Controls.Player.Use.performed -= OnUse;
        Controls.Player.Sprint.performed -= OnSprint;
        Controls.Player.Drop.performed -= OnDrop;
        Controls.Player.Crouch.performed -= OnCrouch;
    }

    private void OnEnable()
    {
        Controls.Player.Enable();
        Controls.Player.Use.performed += OnUse;
        Controls.Player.Sprint.performed += OnSprint;
        Controls.Player.Drop.performed += OnDrop;
        Controls.Player.Crouch.performed += OnCrouch;
    }

    private void OnDrop(InputAction.CallbackContext obj)
    {
        if (InteractableHolding == null)
            return;

        if (InteractableHolding.Interact(this, obj.action))
        {
            InteractableHolding = null;
            SwitchExclusiveInteractions(interactableLookingAt);
        }
    }

    private void OnCrouch(InputAction.CallbackContext obj)
    {
        isCrouching = !isCrouching;
    }

    private void OnSprint(InputAction.CallbackContext obj)
    {
        isRunning = !isRunning;
        currentSpeed = isRunning ? sprintSpeed : moveSpeed;
    }

    private void OnUse(InputAction.CallbackContext obj)
    {
        interactableLookingAt?.Interact(this, obj.action);
    }

    public void PickUp(IInteractable itemToPickUp)
    {
        InteractableHolding = itemToPickUp;
        SwitchExclusiveInteractions(InteractableHolding);
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        CheckGrounded();
        HandleJump();
        HandleInteraction();
    }

    private void HandleJump()
    {
        if (Controls.Player.Jump.IsPressed() && canJump && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(
            new Ray(jumpPoint.position, Vector3.down),
            maxJumpPointDist,
            groundLayerMask
        );
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private void HandleMovement()
    {
        Vector2 moveInput = Controls.Player.Move.ReadValue<Vector2>();

        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
            rb.MovePosition(rb.position + moveDirection * (currentSpeed * Time.deltaTime));
        }
    }

    private float yRotation = 0f;
    private float xRotation = 0f;

    private void HandleLook()
    {
        Vector2 lookInput = Controls.Player.Look.ReadValue<Vector2>();

        yRotation += lookInput.x;
        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, maxLookDownAngle, maxLookUpAngle);

        transform.rotation = Quaternion.Euler(0, yRotation, 0);
        playerCameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
