using System;
using UnityEngine;

public class HintTrigger : MonoBehaviour
{
    [SerializeField]
    private Transform PortTO;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out var player))
        {
            player.transform.SetPositionAndRotation(PortTO.position, Quaternion.identity);
            PlayerUIManager.Instance.NewChapter(2, "TO REGAIN LOST MEMORY");
            Destroy(gameObject);
        }
    }
}