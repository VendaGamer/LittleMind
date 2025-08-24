using UnityEngine;

public partial class PlayerController
{
    [SerializeField] private GameObject playerRoot;
    [SerializeField] private PauseManager pauseMenuManager;
    public void SwitchToPlayer()
    {
        playerRoot.SetActive(true);
        PlayerCamera.Instance.PrioritizePlayerCamera();
    }

    public void SwitchToMainMenu()
    {
        playerRoot.SetActive(false);
        pauseMenuManager.ShowMainMenu();
    }
}