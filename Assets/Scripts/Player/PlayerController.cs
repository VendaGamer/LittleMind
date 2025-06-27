using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviour, IInteractor
{
    private static readonly int IsCrouchingHash = Animator.StringToHash("IsCrouching");
    private static readonly int XVelocityHash = Animator.StringToHash("XVelocity");
    private static readonly int YVelocityHash = Animator.StringToHash("YVelocity");

    [Header("Movement Settings")]
    [SerializeField]
    private float AnimBlendSpeed = 8f;

    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float crouchSpeed = 5f;

    [SerializeField]
    private float playerRotateSpeed = 20f;

    [SerializeField]
    private float maxPlayerBodCamRotDiff = 45f;

    [SerializeField]
    private float sprintSpeed = 7f;

    [SerializeField]
    private float jumpForce = 2f;

    [SerializeField]
    private Transform jumpPoint;

    [SerializeField]
    private float maxJumpPointDist = 0.5f;

    [SerializeField]
    private float jumpCooldown = 0.1f;

    [SerializeField]
    private float jumpPointRadius = 0.3f;

    [SerializeField]
    private LayerMask groundLayerMask;
    
    [SerializeField]
    private CinemachineCamera playerCamera;

    private bool canJump = true;
    private bool isCrouching;
    private bool isClimbing;
    
    private float currentSpeed;
    private Rigidbody rb;
    private bool isRunning;
    private Animator animator;
    
    public static PlayerInput PlayerInput { get; private set; }

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void Start()
    {
        currentSpeed = moveSpeed;
    }

    private void OnDisable()
    {
        var controlsPlayer = InputManager.InputControls.Player;
        controlsPlayer.Disable();
        controlsPlayer.Use.performed -= OnUse;
        controlsPlayer.Sprint.performed -= OnSprint;
        controlsPlayer.Drop.performed -= OnDrop;
        controlsPlayer.Crouch.performed -= OnCrouch;
        
        // TODO: maybe unlock the IK
    }

    private void OnEnable()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        var controlsPlayer = InputManager.InputControls.Player;
        controlsPlayer.Enable();
        controlsPlayer.Use.performed += OnUse;
        controlsPlayer.Sprint.performed += OnSprint;
        controlsPlayer.Drop.performed += OnDrop;
        controlsPlayer.Crouch.performed += OnCrouch;
    }

    private void OnCrouch(InputAction.CallbackContext obj)
    {
        isCrouching = !isCrouching;
        if (isCrouching)
            isRunning = false;
        currentSpeed = moveSpeed;
        animator.SetBool(IsCrouchingHash, isCrouching);
    }

    private void OnSprint(InputAction.CallbackContext obj)
    {
        if (isCrouching)
            return;

        isRunning = !isRunning;
        currentSpeed = isRunning ? sprintSpeed : moveSpeed;
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void Update()
    {
        HandleInteraction();
    }
    
    private void LateUpdate()
    {
        RotatePlayerBody();
    }

    private void HandleJump()
    {
        if (!canJump || !InputManager.InputControls.Player.Jump.IsPressed())
            return;

        if (Physics.SphereCast(new Ray(jumpPoint.position, -transform.up),
                jumpPointRadius, maxJumpPointDist, groundLayerMask))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
            StartCoroutine(JumpCooldown());
        }
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private void RotatePlayerBody()
    {
        float cameraYaw = playerCamera.transform.eulerAngles.y;
        float bodyYaw = transform.eulerAngles.y;

        float angleDiff = Mathf.DeltaAngle(bodyYaw, cameraYaw);

        if (Mathf.Abs(angleDiff) > maxPlayerBodCamRotDiff)
        {
            // Snap body back to be within ±45° of camera
            float correction = angleDiff - Mathf.Sign(angleDiff) * maxPlayerBodCamRotDiff;
            float newBodyYaw = bodyYaw + correction;

            Quaternion newRotation = Quaternion.Euler(0f, newBodyYaw, 0f);
            transform.rotation = newRotation;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            playerRotateSpeed * Time.deltaTime
        );
    }

    private Vector2 currentMoveVelocity;

    private void HandleMovement()
    {
        
        Vector2 moveInput = InputManager.InputControls.Player.Move.ReadValue<Vector2>();

        currentMoveVelocity = Vector2.Lerp(
            currentMoveVelocity,
            moveInput * (isRunning ? 2 : 1),
            AnimBlendSpeed * Time.fixedDeltaTime
        );
        animator.SetFloat(XVelocityHash, currentMoveVelocity.x);
        animator.SetFloat(YVelocityHash, currentMoveVelocity.y);

        // Calculate movement direction relative to camera
        Vector3 moveDirection =
            playerCamera.transform.right * moveInput.x
            + playerCamera.transform.forward * moveInput.y;
        moveDirection.y = 0f;

        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
            rb.MovePosition(rb.position + moveDirection * (currentSpeed * Time.fixedDeltaTime));
        }
    }
}
