using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;


public class PauseManager : MonoBehaviour
{
    [SerializeField]
    private GameObject root;
    [SerializeField] private MenuBase mainMenu;

    [SerializeField] private AudioMenu audioMenu;

    [SerializeField] private VideoMenu videoMenu;
    
    [Tooltip("For player to be able to turn off film grain and so on")]
    [SerializeField]
    private VolumeProfile volumeProfile;
    
    [SerializeField]
    private SettingsHandler settingsHandler;
    
    [SerializeField]
    private InteractionHandler interactionHandler;
    
    public void ToggleVsync(bool value) => settingsHandler.Vsync.SetValue(value);
    public void NextWindowMode() => settingsHandler.WindowMode.NextValue();
    public void PreviousWindowMode() => settingsHandler.WindowMode.PreviousValue();
    public void PreviousResolution() => settingsHandler.Resolution.PreviousValue();
    public void NextResolution() => settingsHandler.Resolution.NextValue();
    public void NextPreset() => settingsHandler.Quality.NextValue();
    public void PreviousPreset() => settingsHandler.Quality.PreviousValue();
    public void CancelSettings() => settingsHandler.ResetVideoSettings();
    public void ApplySettings() => settingsHandler.ApplyVideoSettings();
    
    private MenuBase currentMenu;


    private void Awake()
    {
        settingsHandler.InitValues(mainMenu, audioMenu, videoMenu);
        settingsHandler.Quality.Label = videoMenu.PresetLabel;
        settingsHandler.Resolution.Label = videoMenu.ResolutionLabel;
        settingsHandler.WindowMode.Label = videoMenu.WindowModeLabel;
    }

    private void OnEnable()
    {
        interactionHandler.InputControls.General.Exit.performed += OnExit;
    }
    
    private void OnDisable()
    {
        interactionHandler.InputControls.General.Exit.performed -= OnExit;
        currentMenu = mainMenu;
    }

    public void ShowAudioSettings()
    {
        audioMenu.SetCameraPriority(PlayerCamera.Instance.CurrentVirtualCameraPriority + 1);
        currentMenu = audioMenu;
    }

    public void ShowVideoSettings()
    {
        videoMenu.SetCameraPriority(PlayerCamera.Instance.CurrentVirtualCameraPriority + 1);
        currentMenu = videoMenu;
    }

    private void OnExit(InputAction.CallbackContext obj)
    {
        if (currentMenu == audioMenu || currentMenu == videoMenu)
        {
            ShowVideoSettings();
        }
        else
        {
            root.SetActive(false);
        }
    }
    
    public void SetMusicVolume(float value) => settingsHandler.MusicVolume.SetValue(value);
    public void SetMasterVolume(float value) => settingsHandler.MasterVolume.SetValue(value);
    public void ApplyAudioSettings() => settingsHandler.ApplyAudioSettings();

    public void ResetSoundSettings() => settingsHandler.ResetAudioSettings();
}