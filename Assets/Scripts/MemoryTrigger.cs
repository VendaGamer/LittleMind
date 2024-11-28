using System;
using Unity.VisualScripting;
using UnityEngine;

public class MemoryTrigger : MonoBehaviour
{
    public string memoryId;
    [SerializeField]
    public GameObject[] memoryElements;

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
            if(Input.GetButtonDown("Remember"))
            {
                alzh.
            }
        }
    }
}