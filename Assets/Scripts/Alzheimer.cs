using UnityEngine;

public class Alzheimer : MonoBehaviour
{
    [SerializeField] private Diary playerDiary;
    [SerializeField] private float memoryTriggerDistance = 2f;

    private void TryUnlockMemory(MemoryTrigger trigger)
    {
        if (trigger.entry.isDiscovered)
        {
            trigger.ActivateMemoryElements();
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Journal"))
        {
            playerDiary.gameObject.SetActive(!playerDiary.gameObject.activeSelf);
        }
    }
}