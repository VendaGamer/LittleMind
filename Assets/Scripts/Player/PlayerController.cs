using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField]
    private Transform pickupPoint;
    [SerializeField]
    protected Camera playerCamera;
    [SerializeField]
    private float rayDistance = 3f;
    [SerializeField]
    private float pickupLerpDuration=1f;
    
    
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
    [SerializeField] private List<HintData> globalActions;

    private InteractableObject currentInteractable;
    
    
    private Controls controls;
    private float currentSpeed;
    private Rigidbody rb;
    private bool isRunning = false;
    private PickableObject holding;
    private PickableObject lookingAt;


    protected virtual void Start()
    {
        if(!playerCamera)
            playerCamera = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentSpeed = moveSpeed;
    }

    protected virtual void Awake()
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
        if (holding)
        {
            DropObject();
        }
    }

    private void OnSprint(InputAction.CallbackContext obj)
    {
        isRunning =!isRunning;
        currentSpeed = isRunning ? sprintSpeed : moveSpeed;
    }

    private void OnUse(InputAction.CallbackContext obj)
    {
        if (lookingAt)
        {
            PickUpObject();
        }
    }

    private void PickUpObject()
    {
        holding = lookingAt;
        holding.PickObject(pickupPoint, pickupLerpDuration);
    }
    private void DropObject()
    {
        if (!holding) return;
        holding.DropObject();
        holding = null;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleInteraction();
    }

    private void HandleInteraction()
    {
        // Raycast to check for interactable objects
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.collider.TryGetComponent<InteractableObject>(out var interactable))
            {
                if (currentInteractable != interactable)
                {
                    if (currentInteractable)
                    {
                        hintManager.RefreshAllHints(); // Clear previous hints
                    }

                    currentInteractable = interactable;

                    UpdateHints(currentInteractable.GetContextualHints());
                }
            }
        }
        else if (currentInteractable)
        {
            currentInteractable = null;
            hintManager.RefreshAllHints();
        }
    }
    
    private void UpdateHints(HintData[] hints)
    {
        hintManager.RefreshAllHints();
        foreach (var hint in hints)
        {
            hintManager.AddHint(hint);
        }

        foreach (var globalHint in globalActions)
        {
            hintManager.AddHint(globalHint);
        }
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
