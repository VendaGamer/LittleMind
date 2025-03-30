using System.Collections;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IInteractor
{
    [Header("Pickup Settings")]
    [SerializeField]
    private Transform pickupPoint;
    public Transform PickupPoint=> pickupPoint;
    private static Camera Camera => PlayerCamera.Instance.Camera;
    [SerializeField]
    private float pickupLerpDuration=1f;
    public float PickupLerpDuration => pickupLerpDuration;


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private Transform jumpPoint;
    [SerializeField] private float maxJumpPointDist = 0.2f;
    [SerializeField] private float jumpCooldown = 0.1f;
    [SerializeField] private float jumpPointRadius = 0.3f;
    [SerializeField] private LayerMask groundLayerMask;
    private bool canJump = true;
    private bool isGrounded;
    private bool isCrouching;
    

    [Header("Look Settings")]
    [SerializeField] private float maxLookUpAngle = 90f;
    [SerializeField] private float maxLookDownAngle = -90f;
    [SerializeField] private Transform playerCameraHolder;
    
    [Header("Interaction Settings")]
    [SerializeField] private GlobalInteractions globalInteractionsPlayerControls;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private float sphereCastDistance = 3f;
    [SerializeField] private float sphereCastRadius = 0.1f;
    
    private readonly RaycastHit[] raycastHits = new RaycastHit[1];
    [CanBeNull] private IInteractable interactableLookingAt;
    [CanBeNull] public IInteractable InteractableHolding { get; private set; }

    public static Controls Controls { get; private set; }
    private float currentSpeed;
    private Rigidbody rb;
    private bool isRunning;
    private Animator animator;

    private void Awake()
    {
        Controls = new Controls();
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
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
        if (InteractableHolding == null) return;

        if (InteractableHolding.Interact(this, obj.action))
        {
            InteractableHolding = null;
            SwitchExclusiveInteractions(interactableLookingAt);
        }
    }
    
    private void OnCrouch(InputAction.CallbackContext obj)
    {
        isCrouching =! isCrouching;
        
    }

    private void OnSprint(InputAction.CallbackContext obj)
    {
        isRunning =!isRunning;
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

    private void CheckGrounded()
    {
        isGrounded = Physics.SphereCast(new Ray(jumpPoint.position, Vector3.down), jumpPointRadius,maxJumpPointDist,groundLayerMask);
    }

    private void HandleJump()
    {
        if (Controls.Player.Jump.IsPressed() && isGrounded && canJump)
        {
            rb.AddForce(Vector3.up * (jumpForce * 100f), ForceMode.Force);
            
            StartCoroutine(JumpCooldown());
        }
    }

    private IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    private void HandleInteraction()
    {
        Ray ray = Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, Camera.nearClipPlane));
        int hitCount = Physics.SphereCastNonAlloc(ray, sphereCastRadius, raycastHits, sphereCastDistance, interactableLayerMask);

        if (hitCount > 0)
        {
            RaycastHit hit = raycastHits[0];
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                // If we're looking at a different interactable, update interactions
                if (interactableLookingAt != interactable)
                {
                    
                    if (!interactableLookingAt.IsUnityNull())
                    {
                        interactableLookingAt.ToggleOutline(false);
                        if (interactableLookingAt.CurrentInteractions != interactable.CurrentInteractions)
                        {
                            SwitchExclusiveInteractions(interactable);
                        }
                    }

                    interactable.ToggleOutline(true);
                    interactableLookingAt = interactable;
                }
            }
            else
            {
                // We hit something, but it's not an interactable
                if (!interactableLookingAt.IsUnityNull())
                {
                    interactableLookingAt.ToggleOutline(false);
                    interactableLookingAt = null;
                    SwitchExclusiveInteractions(null);
                }
            }
        }
        else
        {
            // We didn't hit anything
            if (!interactableLookingAt.IsUnityNull())
            {
                interactableLookingAt.ToggleOutline(false);
                interactableLookingAt = null;
                SwitchExclusiveInteractions(null);
            }
        }
    }
    
    private void SwitchGlobalInteractions(GlobalInteractions newGlobalInteractions)
    {
        //uiData.UpdateInteractions(newGlobalInteractions, interactableLookingAt);
    }

    private void SwitchExclusiveInteractions(IInteractable newExclusiveInteractions)
    {
        //uiData.UpdateInteractions(globalInteractionsPlayerControls, newExclusiveInteractions);
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
