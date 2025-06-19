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
    public void NextWindowMode() => settingsHandler.WindowModeSetting.NextValue();
    public void PreviousWindowMode() => settingsHandler.WindowModeSetting.PreviousValue();
    public void PreviousResolution() => settingsHandler.ResolutionSetting.PreviousValue();
    public void NextResolution() => settingsHandler.ResolutionSetting.NextValue();
    public void NextPreset() => settingsHandler.QualitySetting.NextValue();
    public void PreviousPreset() => settingsHandler.QualitySetting.PreviousValue();
    public void CancelSettings() => settingsHandler.ResetVideoSettings();
    public void ApplySettings() => settingsHandler.ApplyVideoSettings();
    
    private MenuBase currentMenu;


    private void Awake()
    {
        settingsHandler.InitValues(mainMenu, audioMenu, videoMenu);
        settingsHandler.QualitySetting.Label = videoMenu.PresetLabel;
        settingsHandler.ResolutionSetting.Label = videoMenu.ResolutionLabel;
        settingsHandler.WindowModeSetting.Label = videoMenu.WindowModeLabel;
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