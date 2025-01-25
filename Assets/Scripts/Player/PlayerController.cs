using System;
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
    private static Camera Camera => PlayerCamera.Camera;
    [SerializeField]
    private float pickupLerpDuration=1f;
    public float PickupLerpDuration => pickupLerpDuration;


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    

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

    public static event Action<GlobalInteractions> GlobalInteractionsChanged;
    public static event Action<IInteractable> ExclusiveInteractableChanged;

    public static Controls Controls { get; private set; }
    private float currentSpeed;
    private Rigidbody rb;
    private bool isRunning = false;

    private void Awake()
    {
        Controls = new Controls();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        currentSpeed = moveSpeed;
        SwitchGlobalInteractions(globalInteractionsPlayerControls);
    }
    
    private void OnDisable()
    {
        Controls.Disable();
        Controls.Player.Use.performed -= OnUse;
        Controls.Player.Sprint.performed -= OnSprint;
        Controls.Player.Drop.performed -= OnDrop;
    }
    
    private void OnEnable()
    {
        Controls.Enable();
        Controls.Player.Use.performed += OnUse;
        Controls.Player.Sprint.performed += OnSprint;
        Controls.Player.Drop.performed += OnDrop;
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
        HandleInteraction();
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
                    if (interactableLookingAt?.CurrentInteractions != interactable.CurrentInteractions)
                    {
                        SwitchExclusiveInteractions(interactable);
                    }
                    interactableLookingAt = interactable;
                }
            }
            else
            {
                // We hit something, but it's not an interactable
                if (!interactableLookingAt.IsUnityNull())
                {
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
                interactableLookingAt = null;
                SwitchExclusiveInteractions(null);
            }
        }
    }

    private void SwitchGlobalInteractions(GlobalInteractions newGlobalInteractions)
    {
        GlobalInteractionsChanged?.Invoke(newGlobalInteractions);
    }

    private void SwitchExclusiveInteractions(IInteractable newExclusiveInteractions)
    {
        ExclusiveInteractableChanged?.Invoke(newExclusiveInteractions);
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
