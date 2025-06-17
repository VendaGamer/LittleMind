using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ZLinq;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

[CreateAssetMenu(menuName = "GameSettings/SettingsHandler", fileName = "SettingsHandler")]
public class SettingsHandler : ScriptableObject
{
    [Header("Quality Presets")]
    [SerializeField] private UniversalRenderPipelineAsset lowQuality, mediumQuality, highQuality;
    
    [Header("Post-Processing")]
    [SerializeField] private VolumeProfile volumeProfile;
    
    [Header("Current Quality")]
    public GraphicsQuality currentQuality = GraphicsQuality.High;

    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup effectMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    
    // Post-Processing Settings
    public BoolSetting filmGrain { get; private set; }
    public BoolSetting bloom { get; private set; }
    public BoolSetting vignette { get; private set; }
    public BoolSetting chromaticAberration { get; private set; }
    public BoolSetting colorGrading { get; private set; }
    public BoolSetting motionBlur { get; private set; }
    public BoolSetting vsync { get; private set; }
    
    // Custom URP Settings
    public DefinedSetting<float> renderScale { get; private set; }
    public DefinedSetting<MsaaQuality> msaaQuality { get; private set; }
    public DefinedSetting<float> ShadowDistance { get; private set; }
    public DefinedSetting<ShadowResolution> ShadowResolution { get; private set; }
    public DefinedSetting<GraphicsQuality> Quality { get; private set; }
    
    // Display Settings
    public DefinedSetting<Resolution> resolutions { get; private set; }
    public DefinedSetting<FullScreenMode> screenModes { get; private set; }
    
    // Texture and Performance Settings
    public DefinedSetting<int> TextureQuality { get; private set; }
    public DefinedSetting<int> LODBias { get; private set; }
    public ValueSetting<int> TargetFramerate { get; private set; }
    public ValueSetting<float> MasterVolume { get; private set; }
    public ValueSetting<float> MusicVolume { get; private set; }
    public ValueSetting<float> EffectsVolume { get; private set; }
    
    private UniversalRenderPipelineAsset customURPAsset;

    public void ApplyAudioSettings()
    {
        MasterVolume.Apply();
        MusicVolume.Apply();
        EffectsVolume.Apply();
    }

    public void ResetAudioSettings()
    {
        MasterVolume.Reset();
        MusicVolume.Reset();
        EffectsVolume.Reset();
    }

    public void ApplyVideoSettings()
    {
        
        // Apply display settings
        resolutions.Apply();
        screenModes.Apply();
        
        // Apply graphics quality
        Quality.Apply();
        currentQuality = Quality.CurrentValue;
        
        // Apply URP settings
        ApplyURPSettings();
        
        // Apply post-processing
        ApplyVolumeSettings();
        
        // Apply other settings
        vsync.Apply();
        TargetFramerate.Apply();
        TextureQuality.Apply();
        LODBias.Apply();
    }

    public void ResetVideoSettings()
    {
        
        resolutions.Reset();
        screenModes.Reset();
        filmGrain.Reset();
        bloom.Reset();
        vignette.Reset();
        chromaticAberration.Reset();
        colorGrading.Reset();
        motionBlur.Reset();
        renderScale.Reset();
        msaaQuality.Reset();
        ShadowDistance.Reset();
        ShadowResolution.Reset();
        Quality.Reset();
        TextureQuality.Reset();
        LODBias.Reset();
        vsync.Reset();
        TargetFramerate.Reset();
    }

    public void InitValues(MenuBase mainMenu, AudioMenu audioMenu, VideoMenu videoMenu)
    {
        // Display Settings
        screenModes = new DefinedSetting<FullScreenMode>(
            new[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed },
            SetResolution,
            Screen.fullScreenMode
        );
            
        var availableResolutions = Screen.resolutions.AsValueEnumerable()
            .Where(r => r.refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
            .OrderBy(r => r.width)
            .ToArray();
            
        resolutions = new DefinedSetting<Resolution>(
            availableResolutions,
            SetResolution,
            Screen.currentResolution);
        
        // Graphics Quality
        Quality = new DefinedSetting<GraphicsQuality>(
            Enum.GetValues(typeof(GraphicsQuality)).AsValueEnumerable().Cast<GraphicsQuality>().ToArray(),
            () => { currentQuality = Quality.CurrentValue; },
            GraphicsQuality.High
        );
        
        // Post-Processing Settings
        filmGrain = new BoolSetting(ApplyVolumeSettings, true);
        bloom = new BoolSetting(ApplyVolumeSettings, true);
        vignette = new BoolSetting(ApplyVolumeSettings, true);
        chromaticAberration = new BoolSetting(ApplyVolumeSettings, true);
        colorGrading = new BoolSetting(ApplyVolumeSettings, true);
        motionBlur = new BoolSetting(ApplyVolumeSettings, false);
        
        // Custom URP Settings
        renderScale = new DefinedSetting<float>(
            new[] { 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 2.0f },
            () => { },
            1.0f
        );
        
        msaaQuality = new DefinedSetting<MsaaQuality>(
            Enum.GetValues(typeof(MsaaQuality)).AsValueEnumerable().Cast<MsaaQuality>().ToArray(),
            () => { },
            MsaaQuality.Disabled
        );
        
        ShadowDistance = new DefinedSetting<float>(
            new[] { 50f, 100f, 150f, 200f, 300f },
            () => { },
            150f
        );
        
        ShadowResolution = new DefinedSetting<ShadowResolution>(
            Enum.GetValues(typeof(ShadowResolution)).AsValueEnumerable().Cast<ShadowResolution>().ToArray(),
            () => { },
            UnityEngine.Rendering.Universal.ShadowResolution._2048
        );
        
        // Performance Settings
        TextureQuality = new DefinedSetting<int>(
            new[] { 3, 2, 1, 0 }, // 0 = full quality, higher = lower quality
            () => QualitySettings.globalTextureMipmapLimit = TextureQuality.CurrentValue,
            0
        );
        
        LODBias = new DefinedSetting<int>(
            new[] { 0, 1, 2 }, // LOD bias levels
            () => QualitySettings.lodBias = LODBias.CurrentValue,
            1
        );
        
        vsync = new BoolSetting(
            () => QualitySettings.vSyncCount = vsync.CurrentValue ? 1 : 0,
            true
        );
    }

    private void SetResolution()
    {
        var resolution = resolutions.CurrentValue;
        var screenMode = screenModes.CurrentValue;
        Screen.SetResolution(resolution.width, resolution.height, screenMode, resolution.refreshRateRatio);
    }



    private UniversalRenderPipelineAsset GetCurrentURPAsset() =>
        currentQuality switch
        {
            GraphicsQuality.Low => lowQuality,
            GraphicsQuality.Medium => mediumQuality,
            GraphicsQuality.High => highQuality,
            _ => mediumQuality
        };
    
    private void ApplyURPSettings()
    {
        var targetAsset = GetCurrentURPAsset();
        
        GraphicsSettings.defaultRenderPipeline = targetAsset;
        QualitySettings.renderPipeline = targetAsset;
    }
    
    private void ApplyCustomURPSettings(UniversalRenderPipelineAsset urpAsset)
    {
        urpAsset.renderScale = renderScale.CurrentValue;
        urpAsset.msaaSampleCount = (int)msaaQuality.CurrentValue;
        urpAsset.shadowDistance = ShadowDistance.CurrentValue;
        urpAsset.mainLightShadowmapResolution = (int)ShadowResolution.CurrentValue;
        urpAsset.additionalLightsShadowmapResolution = (int)ShadowResolution.CurrentValue;
    }
    
    private void ApplyVolumeSettings()
    {
        if (!volumeProfile) return;
        
        ApplyVolumeEffect<FilmGrain>(filmGrain.CurrentValue);
        ApplyVolumeEffect<Bloom>(bloom.CurrentValue);
        ApplyVolumeEffect<Vignette>(vignette.CurrentValue);
        ApplyVolumeEffect<ChromaticAberration>(chromaticAberration.CurrentValue);
        ApplyVolumeEffect<ColorAdjustments>(colorGrading.CurrentValue);
        ApplyVolumeEffect<MotionBlur>(motionBlur.CurrentValue);
    }
    
    private void ApplyVolumeEffect<T>(bool enabled) where T : VolumeComponent
    {
        if (volumeProfile.TryGet<T>(out var effect))
        {
            effect.active = enabled;
        }
    }
    
}

public class BoolSetting
{
    private bool appliedValue;
    private bool currentValue;
    private readonly Action applyAction;

    public bool CurrentValue => currentValue;
    public bool AppliedValue => appliedValue;

    public BoolSetting(Action applyAction, bool initialValue = false)
    {
        this.applyAction = applyAction;
        currentValue = initialValue;
        appliedValue = initialValue;
    }

    public bool Toggle()
    {
        currentValue = !currentValue;
        return currentValue;
    }

    public void SetValue(bool value)
    {
        currentValue = value;
    }

    public void Apply()
    {
        if (CanApplyOrReset)
        {
            applyAction?.Invoke();
            appliedValue = currentValue;
        }
    }

    public bool CanApplyOrReset => appliedValue != currentValue;

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            currentValue = appliedValue;
        }
    }

    public override string ToString() => currentValue ? "Enabled" : "Disabled";
}

public class ValueSetting<T> : ISetting<T> where T : struct
{
    private T currentValue;
    private readonly Action applyAction;

    public T AppliedValue { get; }
    public bool CanApplyOrReset => !currentValue.Equals(AppliedValue);

    public T CurrentValue => currentValue;

    public void SetValue(T value)
    {
        if (!currentValue.Equals(value))
        {
            label.text = value.ToString();
        }
    }

    private TMP_Text label;
    
    public TMP_Text Label
    {
        get => label;
        set
        {
            label = value;
            label.text = ToString();
        }
    }

    public ValueSetting(Action applyAction, T initialValue = default)
    {
        this.applyAction = applyAction;
        currentValue = initialValue;
        AppliedValue = currentValue;
    }

    public void Apply() => applyAction();

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            currentValue = AppliedValue;
        }
    }

}

public interface ISetting<out T> where T : struct
{
    public T CurrentValue { get; }
    public T AppliedValue { get; }
    
    public bool CanApplyOrReset { get; }

    public void Apply();

    public void Reset();
}

public class DefinedSetting<T> : ISetting<T> where T : struct
{
    private int appliedIndex;
    private int currentIndex;

    private TMP_Text label;
    public TMP_Text Label
    {
        get => label;
        set
        {
            label = value;
            label.text = ToString();
        }
    }
    public T CurrentValue => possibleValues[currentIndex];
    public T AppliedValue => possibleValues[appliedIndex];
    private readonly T[] possibleValues;
    private readonly Action applyAction;
    
    [CanBeNull] private readonly Func<string> toStringOverride;

    public DefinedSetting(T[] possibleValues, Action applyAction, T initialValue = default)
    {
        this.possibleValues = possibleValues ?? throw new ArgumentNullException(nameof(possibleValues));
        this.applyAction = applyAction;
        currentIndex = Array.IndexOf(possibleValues, initialValue);
        if (currentIndex < 0) currentIndex = 0;
        
        appliedIndex = currentIndex;
    }

    public T NextValue()
    {
        currentIndex++;
        if (currentIndex > possibleValues.Length - 1)
        {
            currentIndex = 0;
        }

        Label.text = ToString();
        return CurrentValue;
    }

    public T PreviousValue()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = possibleValues.Length - 1;
        }
        Label.text = ToString();
        return CurrentValue;
    }

    public void SetCurrentIndex(int index)
    {
        if (index >= 0 && index < possibleValues.Length)
            currentIndex = index;
    }

    public void Apply()
    {
        if (CanApplyOrReset)
        {
            applyAction?.Invoke();
            appliedIndex = currentIndex;
        }
    }

    public bool CanApplyOrReset => appliedIndex != currentIndex;

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            currentIndex = appliedIndex;
        }
    }

    public override string ToString() => CurrentValue.ToString();
}