using System;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class SubwayMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float runSpeed = 10f;
    public float laneDistance = 7f;
    public float laneChangeDur = 0.8f;

    private Rigidbody rb;
    private int currentLane = 0; // -1 = left, 0 = middle, 1 = right
    private bool isChangingLanes;

    [SerializeField]
    private InteractionHandler interactionHandler;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        interactionHandler.InputControls.Player.Enable();
        interactionHandler.InputControls.Player.Move.performed += OnMove;
    }

    private void OnDisable()
    {
        interactionHandler.InputControls.Player.Disable();
        interactionHandler.InputControls.Player.Move.performed -= OnMove;
    }

    void FixedUpdate()
    {
        // Constant forward movement (considering 270° Y rotation)
        Vector3 forwardMovement = transform.forward * (runSpeed * Time.deltaTime);

        rb.MovePosition(rb.position + forwardMovement);
    }

    private void OnMove(CallbackContext context)
    {
        if (isChangingLanes)
            return;

        Vector2 input = context.ReadValue<Vector2>();

        if (Mathf.Abs(input.x) > 0.5f)
        {
            int direction = input.x > 0 ? 1 : -1;
            int newLane = Mathf.Clamp(currentLane + direction, -1, 1);

            if (newLane != currentLane)
            {
                currentLane = newLane;

                Vector3 laneOffset = transform.right * (currentLane * laneDistance);
                rb.DOMove(transform.position + laneOffset, laneChangeDur)
                    .OnComplete(() => isChangingLanes = false);
                isChangingLanes = true;
            }
        }
    }
}
