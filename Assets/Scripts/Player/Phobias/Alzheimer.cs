using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Alzheimer : MonoBehaviour
{
    [SerializeField]
    private Diary playerDiary;

    [CanBeNull]
    private MemoryTrigger currentMemoryTrigger;

    private void Start()
    {
        PlayerController.Controls.Player.Journal.performed += OnJournal;
    }

    private void OnEnable()
    {
        if (PlayerController.Controls != null)
        {
            PlayerController.Controls.Player.Journal.performed += OnJournal;
        }
    }

    private void OnDisable()
    {
        PlayerController.Controls.Player.Journal.performed -= OnJournal;
    }

    private void OnJournal(InputAction.CallbackContext obj)
    {
        playerDiary.NegateActiveState();
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
