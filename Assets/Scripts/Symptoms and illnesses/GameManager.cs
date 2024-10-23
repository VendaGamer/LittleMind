using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;

    void Start()
    {
        FPSController playerComponent = player.GetComponent<FPSController>();
    }
}