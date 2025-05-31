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
    private Transform playerCameraHolder;

    public static Controls Controls { get; private set; }
    private float currentSpeed;
    private Rigidbody rb;
    private bool isRunning;
    private Animator animator;
    
    private PlayerInput playerInput;

    private void Awake()
    {
        Controls ??= new Controls();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = moveSpeed;
    }

    private void OnDisable()
    {
        Controls.Player.Disable();
        Controls.Player.Use.performed -= OnUse;
        Controls.Player.Sprint.performed -= OnSprint;
        Controls.Player.Drop.performed -= OnDrop;
        Controls.Player.Crouch.performed -= OnCrouch;
        playerInput.onControlsChanged -= interactionHandler.OnControlsChanged;
    }

    private void OnEnable()
    {
        Controls.Player.Enable();
        Controls.Player.Use.performed += OnUse;
        Controls.Player.Sprint.performed += OnSprint;
        Controls.Player.Drop.performed += OnDrop;
        Controls.Player.Crouch.performed += OnCrouch;
        playerInput.onControlsChanged += interactionHandler.OnControlsChanged;
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

    private void Update()
    {
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

        Vector3 moveDirection =
            playerCamera.transform.right * moveInput.x
            + playerCamera.transform.forward * moveInput.y;
        moveDirection.y = 0;

        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
            rb.MovePosition(rb.position + moveDirection * (currentSpeed * Time.deltaTime));
        }
    }
}
