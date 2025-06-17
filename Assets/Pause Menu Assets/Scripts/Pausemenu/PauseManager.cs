using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;


public class PauseManager : MonoBehaviour
{
    [Header("UI Panels")]
    [Tooltip("Main panel holder that contains the pause menu")]
    [SerializeField]
    private GameObject mainPanel;

    [Tooltip("Video panel holder that contains video settings")]
    [SerializeField]
    private GameObject vidPanel;

    [Tooltip("Audio panel holder that contains audio sliders")]
    [SerializeField]
    private GameObject audioPanel;

    [Tooltip("Game objects with title texts like 'Pause menu' and 'Game Title'")]
    [SerializeField]
    private GameObject TitleTexts;

    [Tooltip("The mask that darkens the scene")]
    [SerializeField]
    private GameObject mask;

    [Header("Animators")]
    [SerializeField]
    private Animator audioPanelAnimator;

    [SerializeField]
    private Animator vidPanelAnimator;

    [SerializeField]
    private Animator quitPanelAnimator;

    [Header("UI Elements")]
    [SerializeField]
    private Text pauseMenu;

    [Header("Scene References")]
    [Tooltip("Scene name for the main menu (example: 'mainmenu')")]
    [SerializeField]
    private string mainMenu;
    
    [Tooltip("For player to be able to turn off film grain and so on")]
    [SerializeField]
    private VolumeProfile volumeProfile;

    [Header("Terrain Settings")]
    [Tooltip("Terrain detail density - adjustable in editor")]
    [SerializeField]
    private float detailDensity;

    [Header("Game Settings")]
    [Tooltip("Default timescale value (1 is normal speed)")]
    [SerializeField]
    private float timeScale = 1f;
    
    [SerializeField]
    private SettingsHandler settingsHandler;
    
    [Header("UI Controls - Video")]
    [SerializeField]
    private Dropdown aaCombo;

    [SerializeField]
    private Dropdown afCombo;

    [SerializeField]
    private Slider fovSlider;

    [SerializeField]
    private Slider modelQualSlider;

    [SerializeField]
    private Slider terrainQualSlider;

    [SerializeField]
    private Slider highQualTreeSlider;

    [SerializeField]
    private Slider renderDistSlider;

    [SerializeField]
    private Slider terrainDensitySlider;

    [SerializeField]
    private Slider shadowDistSlider;

    [SerializeField]
    private Slider masterTexSlider;

    [SerializeField]
    private Slider shadowCascadesSlider;

    [SerializeField]
    private Toggle vSyncToggle;

    [SerializeField]
    private Toggle aoToggle;

    [SerializeField]
    private Toggle dofToggle;

    [SerializeField]
    private Toggle fullscreenToggle;

    [Header("UI Controls - Audio")]
    [SerializeField]
    private Slider audioMasterSlider;

    [SerializeField]
    private Slider audioMusicSlider;

    [SerializeField]
    private Slider audioEffectsSlider;

    [Header("UI Text Elements")]
    [SerializeField]
    private Text presetLabel;

    [SerializeField]
    private Text resolutionLabel;
    
    [SerializeField]
    private Text windowModeLabel;
    

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource[] music;

    [SerializeField]
    private AudioSource[] effects;


    private void Awake()
    {
        settingsHandler.InitValues();
        settingsHandler.qualitySetting.Label = presetLabel;
        settingsHandler.resolutions.Label = resolutionLabel;
        settingsHandler.screenModes.Label = windowModeLabel;
    }
    public void ToggleVsync(bool value) => settingsHandler.vsync.SetValue(value);

    public void NextWindowMode() => settingsHandler.screenModes.NextValue();
    public void PreviousWindowMode() => settingsHandler.screenModes.PreviousValue();

    public void PreviousResolution() => settingsHandler.resolutions.PreviousValue();

    public void NextResolution() => settingsHandler.resolutions.NextValue();
    
    
}
