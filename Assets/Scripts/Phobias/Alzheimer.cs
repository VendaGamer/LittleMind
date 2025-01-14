using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Alzheimer : MonoBehaviour
{
    [SerializeField] private Diary playerDiary;
    [SerializeField] private float memoryTriggerDistance = 2f;
    [CanBeNull]private MemoryTrigger currentMemoryTrigger;
    private Controls playerControls => PlayerController.Controls;

    private void OnEnable()
    {
        playerControls.Player.Journal.performed += OnJournal;
    }

    private void OnJournal(InputAction.CallbackContext obj)
    {
        playerDiary.NegateActiveState();
    }


    private void OnDisable()
    {
        playerControls.Player.Journal.performed -= OnJournal;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MemoryTrigger>(out var trigger))
        {
            currentMemoryTrigger = trigger;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MemoryTrigger>(out var trigger))
        {
            currentMemoryTrigger = null;
        }
    }
}