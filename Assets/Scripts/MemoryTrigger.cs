using UnityEngine;

public class MemoryTrigger : MonoBehaviour
{
    public string memoryId;
    public GameObject[] memoryElements;

    public void ActivateMemoryElements()
    {
        foreach (GameObject element in memoryElements)
        {
            element.SetActive(true);
        }
    }
}