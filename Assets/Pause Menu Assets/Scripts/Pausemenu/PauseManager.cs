using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.UI;
using ZLinq;


public class PauseManager : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;

    [SerializeField] private AudioMenu audioMenu;

    [SerializeField] private VideoMenu videoMenu;
    
    
    [Tooltip("For player to be able to turn off film grain and so on")]
    [SerializeField]
    private VolumeProfile volumeProfile;
    
    [SerializeField]
    private SettingsHandler settingsHandler;
    

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource[] music;

    [SerializeField]
    private AudioSource[] effects;


    private void Awake()
    {
        settingsHandler.InitValues();

        music = GetAudioSourcesByTag("Audio_music");

        effects = GetAudioSourcesByTag("Audio_effect");
        
        settingsHandler.Quality.Label = videoMenu.PresetLabel;
        settingsHandler.resolutions.Label = videoMenu.ResolutionLabel;
        settingsHandler.screenModes.Label = videoMenu.WindowModeLabel;
        
        
        
    }
    
    public AudioSource[] GetAudioSourcesByTag(string tag) =>
        GameObject.FindGameObjectsWithTag(tag)
            .AsValueEnumerable()
            .Select(g => g.GetComponent<AudioSource>())
            .ToArray();
        
    public void ToggleVsync(bool value) => settingsHandler.vsync.SetValue(value);

    public void NextWindowMode() => settingsHandler.screenModes.NextValue();
    public void PreviousWindowMode() => settingsHandler.screenModes.PreviousValue();

    public void PreviousResolution() => settingsHandler.resolutions.PreviousValue();

    public void NextResolution() => settingsHandler.resolutions.NextValue();

    public void NextPreset() => settingsHandler.Quality.NextValue();
    
    public void PreviousPreset() => settingsHandler.Quality.PreviousValue();

    public void CancelSettings() => settingsHandler.ResetSettings();

    public void ApplySettings() => settingsHandler.ApplySettings();

    private SliderSetting<float> masterVolume = new(() =>
        {
            
        });
    private SliderSetting<float> musicVolume;
    private SliderSetting<float> effectsVolume;
    public void SetMusicVolume(float value) => musicVolume.SetValue(value);
    public void SetMasterVolume(float value) => masterVolume.SetValue(value);
    public void ApplySoundSettings()
    {
        musicVolume.Apply();
        effectsVolume.Apply();
        masterVolume.Apply();
    }

    public void ResetSoundSettings()
    {
        musicVolume.Reset();
        effectsVolume.Reset();
        masterVolume.Reset();
    }
    

}
[Serializable]
public class MenuBase
{
    [Header("Root object of menu should be panel")]
    private GameObject root;

    public void SetActive(bool value) => root.SetActive(value);

}
[Serializable]
public class PauseMenu : MenuBase
{
    public TMP_Text Title;
}

[Serializable]
public class AudioMenu : MenuBase
{
    [Header("Audio Menu text fields")]
    [field:SerializeField]
    public TMP_Text MasterVolValueLabel { get; private set; }

    [field:SerializeField]
    public TMP_Text MusicVolValueLabel { get; private set; }
    
    [field:SerializeField]
    public TMP_Text EffectVolValueLabel { get; private set; }
    
    [Header("Sliders text fields")]
    [field:SerializeField]
    public Slider MasterVolSlider { get; private set; }
    
    [field:SerializeField]
    public Slider MusicVolSlider { get; private set; }
    
    [field:SerializeField]
    public Slider EffectVolSlider { get; private set; }
}
[Serializable]
public class VideoMenu : MenuBase
{
    [Header("Video menu text fields")]
    [field:SerializeField]
    public TMP_Text PresetLabel { get; private set; }

    [field:SerializeField]
    public TMP_Text ResolutionLabel { get; private set; }
    
    [field:SerializeField]
    public TMP_Text WindowModeLabel { get; private set; }
}