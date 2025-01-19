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
    [SerializeField]
    protected Camera playerCamera;
    [SerializeField]
    private float rayDistance = 3f;
    [SerializeField]
    private float pickupLerpDuration=1f;
    public float PickupLerpDuration => pickupLerpDuration;


    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 7f;
    

    [Header("Look Settings")]
    [SerializeField] private float maxLookUpAngle = 90f;
    [SerializeField] private float maxLookDownAngle = -90f;
    [SerializeField]private Transform playerCameraHolder;
    
    [Header("Interaction Settings")]
    [SerializeField] private GlobalInteractions globalInteractionsPlayerControls;

    [CanBeNull]private IInteractable interactableLookingAt;
    [CanBeNull]private IInteractable interactableHolding;
    public IInteractable InteractableHolding => interactableHolding;
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
        if(!playerCamera)
            playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        
        Cursor.lockState = CursorLockMode.Locked;
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
        if (interactableHolding == null) return;

        if (interactableHolding.Interact(this, obj.action))
        {
            interactableHolding = null;
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
        interactableHolding = itemToPickUp;
        SwitchExclusiveInteractions(interactableHolding);
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        // Early return if we're looking at the same object we're holding
        if(interactableLookingAt == interactableHolding && !interactableHolding.IsUnityNull())
            return;
    
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
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

    private void HandleLook()
    {
        Vector2 lookInput = Controls.Player.Look.ReadValue<Vector2>();
        
        transform.Rotate(Vector3.up * lookInput.x);


        float newXRotation = playerCameraHolder.localEulerAngles.x - lookInput.y;
        
        if (newXRotation > 180f) newXRotation -= 360f;
        newXRotation = Mathf.Clamp(newXRotation, maxLookDownAngle, maxLookUpAngle);
        
        playerCameraHolder.localEulerAngles = new Vector3(newXRotation, 0f, 0f);
    }
    
}
