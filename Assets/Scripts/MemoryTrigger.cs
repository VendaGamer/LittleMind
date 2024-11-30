using UnityEngine;

public class MemoryTrigger : MonoBehaviour
{
    [SerializeField] public GameObject[] memoryElements { get; }

    [SerializeField] public Diary.DiaryEntry entry;

    public void ActivateMemoryElements()
    {
        foreach (GameObject element in memoryElements)
        {
            element.SetActive(true);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.collider.TryGetComponent<Alzheimer>(out var alzh))
        {
            
        }
    }
}