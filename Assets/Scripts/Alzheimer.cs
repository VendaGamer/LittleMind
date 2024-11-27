using System;
using System.Collections.Generic;
using UnityEngine;

public class Alzheimer : MonoBehaviour
{
    [SerializeField] private Diary playerDiary;
    [SerializeField] private float memoryTriggerDistance = 2f;

    private PlayerController playerController;

    private void Awake()
    {
        playerDiary = new Diary();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        // Check for diary interactions
        if (Input.GetKeyDown(KeyCode.J)) // J for journal/diary
        {
            OpenDiary();
        }
    }

    private void OpenDiary()
    {
        playerDiary.DisplayDiary();
    }

    // Method to add memory trigger when player is near an object
    public void CheckMemoryTriggers()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, memoryTriggerDistance);
        foreach (Collider obj in nearbyObjects)
        {
            MemoryTrigger trigger = obj.GetComponent<MemoryTrigger>();
            if (trigger != null)
            {
                TryUnlockMemory(trigger);
            }
        }
    }

    private void TryUnlockMemory(MemoryTrigger trigger)
    {
        if (playerDiary.HasMemoryNote(trigger.memoryId))
        {
            trigger.ActivateMemoryElements();
        }
    }
}