using UnityEngine;
using UnityEngine.Rendering;
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
    private VolumeProfile volumeProfile;
    
    [SerializeField]
    private SettingsHandler settingsHandler;
    
    [SerializeField]
    private InteractionHandler interactionHandler;
    

    
    
    private MenuBase _currentMenu;

    private MenuBase currentMenu
    {
        set
        {
            _currentMenu?.Hide();
            _currentMenu = value;
            _currentMenu?.Show();
        }
    }
    private PlayerController playerController;


    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        settingsHandler.InitValues(audioMenu, videoMenu);
    }

    private void OnEnable()
    {
        currentMenu = mainMenu;
    }
    
    private void OnDisable()
    {
        currentMenu = null;
    }

    public void ShowAudioSettings()
    {
        currentMenu = audioMenu;
    }

    public void ShowVideoSettings()
    {
        currentMenu = videoMenu;
    }

    private void OnExit(CallbackContext _)
    {
        if (ReferenceEquals(_currentMenu, videoMenu) || ReferenceEquals(_currentMenu, audioMenu))
        {
            currentMenu = mainMenu;
        }
        else
        {
            currentMenu = null;
            root.SetActive(false);
            playerController.SwitchToPlayer();
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
    public void SetBloom(bool value) => settingsHandler.BloomSetting.SetValue(value);
    public void SetMotionBlur(bool value) => settingsHandler.MotionBlurSetting.SetValue(value);
    public void SetChromaticAberration(bool value) => settingsHandler.ChromaticAberrationSetting.SetValue(value);
    public void SetFilmGrain(bool value) => settingsHandler.FilmGrainSetting.SetValue(value);
    public void SetVignette(bool value) => settingsHandler.VignetteSetting.SetValue(value);
    
    
    public void ApplyAudioSettings() => settingsHandler.ApplyAudioSettings();
    public void CancelAudioSettings() => settingsHandler.ResetAudioSettings();
}