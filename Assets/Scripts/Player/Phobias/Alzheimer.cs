using System.Collections.Generic;
using Seagull.Interior_01;
using UnityEngine;
using UnityEngine.InputSystem;

public class Alzheimer : MonoBehaviour
{
    [SerializeField]
    private Diary playerDiary;
    private List<MemoryTrigger> currentMemoryTriggers;
    private void Start()
    {
        PlayerController.Controls.Player.Journal.performed += OnJournal;
    }

    public void RegisterMemoryTrigger(MemoryTrigger trigger)
    {
        PlayerUIManager.Instance.MemoryIconVisibility = true;
        currentMemoryTriggers.Add(trigger);
    }

    public void UnregisterMemoryTrigger(MemoryTrigger trigger)
    {
        if (currentMemoryTriggers.Remove(trigger))
        {
            if (currentMemoryTriggers.Count == 0)
            {
                PlayerUIManager.Instance.MemoryIconVisibility = false;
            }
        }
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
}
