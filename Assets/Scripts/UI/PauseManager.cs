using UnityEngine;
using UnityEngine.Rendering;


public class PauseManager : MonoBehaviour
{
    [SerializeField] private MenuBase mainMenu;

    [SerializeField] private AudioMenu audioMenu;

    [SerializeField] private VideoMenu videoMenu;
    
    [Tooltip("For player to be able to turn off film grain and so on")]
    [SerializeField]
    private VolumeProfile volumeProfile;
    
    [SerializeField]
    private SettingsHandler settingsHandler;
    
    public void ToggleVsync(bool value) => settingsHandler.Vsync.SetValue(value);
    public void NextWindowMode() => settingsHandler.screenModes.NextValue();
    public void PreviousWindowMode() => settingsHandler.screenModes.PreviousValue();
    public void PreviousResolution() => settingsHandler.resolutions.PreviousValue();
    public void NextResolution() => settingsHandler.resolutions.NextValue();
    public void NextPreset() => settingsHandler.Quality.NextValue();
    public void PreviousPreset() => settingsHandler.Quality.PreviousValue();
    public void CancelSettings() => settingsHandler.ResetVideoSettings();
    public void ApplySettings() => settingsHandler.ApplyVideoSettings();


    private void Awake()
    {
        settingsHandler.InitValues(mainMenu, audioMenu, videoMenu);
        settingsHandler.Quality.Label = videoMenu.PresetLabel;
        settingsHandler.resolutions.Label = videoMenu.ResolutionLabel;
        settingsHandler.screenModes.Label = videoMenu.WindowModeLabel;
    }

    public void ShowAudioSettings() =>
        audioMenu.SetCameraPriority(PlayerCamera.Instance.CurrentVirtualCameraPriority + 1);

    public void ShowVideoSettings() =>
        videoMenu.SetCameraPriority(PlayerCamera.Instance.CurrentVirtualCameraPriority + 1);
    public void SetMusicVolume(float value) => settingsHandler.MusicVolume.SetValue(value);
    public void SetMasterVolume(float value) => settingsHandler.MasterVolume.SetValue(value);
    public void ApplyAudioSettings() => settingsHandler.ApplyAudioSettings();

    public void ResetSoundSettings() => settingsHandler.ResetAudioSettings();
}