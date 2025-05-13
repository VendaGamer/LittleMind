using System.Collections.Generic;
using JetBrains.Annotations;
using Seagull.Interior_01;
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

    public void RegisterMemoryTrigger(MemoryTrigger trigger)
    {
        Debug.Log("Alzheimer: Registering memory trigger");
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
        
        if (GeometryUtility.TestPlanesAABB(PlayerCamera.Instance.FrustumPlanes, currentMemoryTrigger.BoundsToLookAt))
        {
            PlayerUIManager.Instance.MemoryIconVisibility = true;
        }
        else
        {
            PlayerUIManager.Instance.MemoryIconVisibility = false;
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
