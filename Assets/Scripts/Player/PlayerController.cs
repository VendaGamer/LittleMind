using System;
using System.Collections.Generic;
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
    [SerializeField] private float mouseSensitivity = 0.2f;
    [SerializeField]private Transform playerCameraHolder;
    
    [Header("Interaction Settings")]
    [SerializeField] private HintManager hintManager;
    [SerializeField] private Interaction[] globalInteractionsGameplay;

    [CanBeNull]private IInteractable interactableLookingAt;
    [CanBeNull]private IInteractable interactableHolding;
    public static event Action<Interaction[]> GlobalInteractionsChanged;
    public static event Action<Interaction[]> ExclusiveInteractionsChanged;
    
    
    private Controls controls;
    private float currentSpeed;
    private Rigidbody rb;
    private bool isRunning = false;


    private void Start()
    {
        if(!playerCamera)
            playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = moveSpeed;
    }

    private void Awake()
    {
        controls = new Controls();
    }
    
    private void OnDisable()
    {
        controls.Disable();
        controls.Player.Use.performed -= OnUse;
        controls.Player.Sprint.performed -= OnSprint;
        controls.Player.Drop.performed -= OnDrop;
    }
    
    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Use.performed += OnUse;
        controls.Player.Sprint.performed += OnSprint;
        controls.Player.Drop.performed += OnDrop;
    }

    private void OnDrop(InputAction.CallbackContext obj)
    {
        if (interactableHolding == null) return;
        
        interactableHolding.Interact(this, obj.action);
        interactableHolding = null;
    }

    private void OnSprint(InputAction.CallbackContext obj)
    {
        isRunning =!isRunning;
        currentSpeed = isRunning ? sprintSpeed : moveSpeed;
    }

    private void OnUse(InputAction.CallbackContext obj)
    {
        if (interactableLookingAt == null)
        {
            return;
        }

        var result = interactableLookingAt.Interact(this, obj.action);
        if (result)
        {
            interactableHolding = interactableLookingAt;
        }
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        if(interactableLookingAt == interactableHolding)return;
        
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                if (interactableLookingAt == interactable) return;

                if (interactableLookingAt?.Interactions == interactable.Interactions)
                {
                    SwitchExclusiveInteractions(interactable.Interactions);
                }
                interactableLookingAt = interactable;
            }
        }
    }

    private void SwitchGlobalInteractions(Interaction[] newGlobalInteractions)
    {
        GlobalInteractionsChanged?.Invoke(newGlobalInteractions);
    }

    private void SwitchExclusiveInteractions(Interaction[] newExclusiveInteractions)
    {
        ExclusiveInteractionsChanged?.Invoke(newExclusiveInteractions);
    }
    
    private void HandleMovement()
    {
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();
        
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        
        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();
            rb.MovePosition(rb.position + moveDirection * (currentSpeed * Time.deltaTime));
        }
    }

    private void HandleLook()
    {
        Vector2 lookInput = controls.Player.Look.ReadValue<Vector2>() * mouseSensitivity;
        
        transform.Rotate(Vector3.up * lookInput.x, Space.Self);


        float newXRotation = playerCameraHolder.localEulerAngles.x - lookInput.y;
        
        if (newXRotation > 180f) newXRotation -= 360f;
        newXRotation = Mathf.Clamp(newXRotation, maxLookDownAngle, maxLookUpAngle);
        
        playerCameraHolder.localEulerAngles = new Vector3(newXRotation, 0f, 0f);
    }
    
}
