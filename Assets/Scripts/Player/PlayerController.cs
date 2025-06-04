using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class PlayerController : MonoBehaviour, IInteractor
{
    [Header("Movement Settings")]
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField] 
    private float playerRotateSpeed = 20f;

    [SerializeField] 
    private float maxPlayerBodCamRotDiff = 45f;
    
    [SerializeField]
    private Transform playerBodyTransform;

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
    
    private float currentSpeed;
    private Rigidbody rb;
    private bool isRunning;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentSpeed = moveSpeed;
    }

    private void OnDisable()
    {
        var controlsPlayer = interactionHandler.InputControls.Player;
        controlsPlayer.Disable();
        controlsPlayer.Use.performed -= OnUse;
        controlsPlayer.Sprint.performed -= OnSprint;
        controlsPlayer.Drop.performed -= OnDrop;
        controlsPlayer.Crouch.performed -= OnCrouch;
    }

    private void OnEnable()
    {
        var controlsPlayer = interactionHandler.InputControls.Player;
        controlsPlayer.Enable();
        controlsPlayer.Use.performed += OnUse;
        controlsPlayer.Sprint.performed += OnSprint;
        controlsPlayer.Drop.performed += OnDrop;
        controlsPlayer.Crouch.performed += OnCrouch;
        interactionHandler.SetGlobalInteractions(globalInteractionGroupPlayerControls);
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
        InteractableLookingAt?.Interact(this, obj.action);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        CheckGrounded();
        HandleJump();
        HandleInteraction();
    }

    private void HandleJump()
    {
        if (interactionHandler.InputControls.Player.Jump.IsPressed() && canJump && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
            StartCoroutine(JumpCooldown());
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


    private void LateUpdate()
    {
        RotatePlayerBody();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RotatePlayerBody()
    {
        float cameraYaw = playerCamera.transform.eulerAngles.y;
        float bodyYaw = playerBodyTransform.eulerAngles.y;

        float angleDiff = Mathf.DeltaAngle(bodyYaw, cameraYaw);

        if (Mathf.Abs(angleDiff) > maxPlayerBodCamRotDiff)
        {
            // Snap body back to be within ±45° of camera
            float correction = angleDiff - Mathf.Sign(angleDiff) * maxPlayerBodCamRotDiff;
            float newBodyYaw = bodyYaw + correction;

            Quaternion newRotation = Quaternion.Euler(0f, newBodyYaw, 0f);
            playerBodyTransform.rotation = newRotation;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);
        playerBodyTransform.rotation = Quaternion.Slerp(
            playerBodyTransform.rotation,
            targetRotation,
            playerRotateSpeed * Time.deltaTime
        );
    }

    private void HandleMovement()
    {
        Vector2 moveInput = interactionHandler.InputControls.Player.Move.ReadValue<Vector2>();

        // Calculate movement direction relative to camera
        Vector3 moveDirection =
            playerCamera.transform.right * moveInput.x +
            playerCamera.transform.forward * moveInput.y;
        moveDirection.y = 0f;

        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
            rb.MovePosition(rb.position + moveDirection * (currentSpeed * Time.deltaTime));
        }
    }
    
}
