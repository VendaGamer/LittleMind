using System;
using JetBrains.Annotations;
using UnityEngine;

public class Alzheimer : MonoBehaviour
{
    [SerializeField] private Diary playerDiary;
    [SerializeField] private float memoryTriggerDistance = 2f;
    [CanBeNull]private MemoryTrigger currentMemoryTrigger;

    private void Update()
    {
        // Toggle the diary display
        if (Input.GetButtonDown("Journal"))
        {
            playerDiary.gameObject.SetActive(!playerDiary.gameObject.activeSelf);
        }

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