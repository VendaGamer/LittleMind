using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    [SerializeField]


    void Start()
    {
        FPSController playerComponent = player.GetComponent<FPSController>();
    }
}