using UnityEngine;

public partial class PlayerController
{
    [SerializeField] private PauseManager pauseMenuManager;

    public void SwitchToMainMenu()
    {
        pauseMenuManager.ShowMainMenu();
    }
}