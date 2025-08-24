
using UnityEngine;

public class RevealTrigger : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var playerController))
        {
            Diary.Instance.UnlockNextNote();
            Destroy(gameObject);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}