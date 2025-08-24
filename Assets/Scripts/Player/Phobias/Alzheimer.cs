using JetBrains.Annotations;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class Alzheimer : MonoBehaviour
{
    [SerializeField]
    private Diary playerDiary;

    [CanBeNull]
    private MemoryTrigger currentMemoryTrigger;

    public void RegisterMemoryTrigger(MemoryTrigger trigger)
    {
        if (ReferenceEquals(trigger, currentMemoryTrigger))
        {
            return;
        }
        
        currentMemoryTrigger = trigger;
    }

    public void UnregisterMemoryTrigger(MemoryTrigger trigger)
    {
        if (ReferenceEquals(trigger, currentMemoryTrigger))
        {
            currentMemoryTrigger = null;
        }
    }

    private void Update()
    {
        if (!currentMemoryTrigger)
        {
            return;
        }
    }

    private void OnEnable()
    {
        InputManager.InputControls.Player.Journal.performed += OnJournal;
    }

    private void OnDisable()
    {
        InputManager.InputControls.Player.Journal.performed -= OnJournal;
    }

    private void OnJournal(CallbackContext _)
    {
        playerDiary.NegateActiveState();
    }
}
