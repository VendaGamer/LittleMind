using JetBrains.Annotations;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PauseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject root;
    
    [SerializeField]
    private MenuBase mainMenu;

    [SerializeField] 
    private AudioMenu audioMenu;

    [SerializeField] 
    private VideoMenu videoMenu;
    
    [SerializeField]
    private SettingsHandler settingsHandler;
    
    private MenuBase _currentMenu;
    
    [SerializeField]
    private GameObject playerRoot;
    
    
    private void Awake()
    {
        settingsHandler.InitValues(audioMenu, videoMenu);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        TransitionToMenu(mainMenu);
        InputManager.InputControls.General.Exit.performed += OnExit;
        PlayerUIManager.Instance.Visibility = false;
    }
    
    private void OnDisable()
    {
        TransitionToMenu(null);
        InputManager.InputControls.General.Exit.performed -= OnExit;
        PlayerUIManager.Instance.Visibility = true;
    }

    public void ShowAudioSettings()
    {
        TransitionToMenu(audioMenu);
    }

    public void ShowVideoSettings()
    {
        TransitionToMenu(videoMenu);
    }

    public void ShowMainMenu()
    {
        TransitionToMenu(mainMenu);
        root.SetActive(true);
        playerRoot.SetActive(false);
    }

    private void TransitionToMenu([CanBeNull] MenuBase menu)
    {
        _currentMenu?.Hide();
        _currentMenu = menu;
        _currentMenu?.Show();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ExitMenu()
    {
        TransitionToMenu(null);
        root.SetActive(false);
        playerRoot.SetActive(true);
    }

    private void OnExit(CallbackContext ctx)
    {
        if (ReferenceEquals(_currentMenu, audioMenu) || ReferenceEquals(_currentMenu, videoMenu))
        {
            TransitionToMenu(mainMenu);
        }
        else
        {
            ExitMenu();
        }
    }
    
    //video settings - general
    public void NextVsync() => settingsHandler.VsyncSetting.NextValue();
    public void NextWindowMode() => settingsHandler.WindowModeSetting.NextValue();
    public void NextResolution() => settingsHandler.ResolutionSetting.NextValue();
    public void NextPreset() => settingsHandler.QualitySetting.NextValue();
    public void NextAntialiasingMode() => settingsHandler.AntialiasingModeSetting.NextValue();
    public void NextAntialiasingQuality() => settingsHandler.AntialiasingQualitySetting.NextValue();
    
    public void PreviousVsync() => settingsHandler.VsyncSetting.PreviousValue();
    public void PreviousWindowMode() => settingsHandler.WindowModeSetting.PreviousValue();
    public void PreviousResolution() => settingsHandler.ResolutionSetting.PreviousValue();
    public void PreviousAntialiasingMode() => settingsHandler.AntialiasingModeSetting.PreviousValue();
    public void PreviousPreset() => settingsHandler.QualitySetting.PreviousValue();
    public void PreviousAntialiasingQuality() => settingsHandler.AntialiasingQualitySetting.PreviousValue();
    
    public void ApplyVideoSettings() => settingsHandler.ApplyVideoSettings();
    public void CancelVideoSettings() => settingsHandler.ResetVideoSettings();
    

    //video settings - post process
    
    
    public void ApplyAudioSettings() => settingsHandler.ApplyAudioSettings();
    public void CancelAudioSettings() => settingsHandler.ResetAudioSettings();
}