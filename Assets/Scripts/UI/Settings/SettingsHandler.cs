using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
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

    // Display Settings
    public SettingsValue<Resolution> resolutions { get; private set; }
    public SettingsValue<FullScreenMode> screenModes { get; private set; }
    
    // Post-Processing Settings
    public BoolSetting filmGrain { get; private set; }
    public BoolSetting bloom { get; private set; }
    public BoolSetting vignette { get; private set; }
    public BoolSetting chromaticAberration { get; private set; }
    public BoolSetting colorGrading { get; private set; }
    public BoolSetting motionBlur { get; private set; }
    public BoolSetting vsync { get; private set; }
    
    // Custom URP Settings
    public SettingsValue<float> renderScale { get; private set; }
    public SettingsValue<MsaaQuality> msaaQuality { get; private set; }
    public SettingsValue<float> shadowDistance { get; private set; }
    public SettingsValue<ShadowResolution> shadowResolution { get; private set; }
    public SettingsValue<GraphicsQuality> qualitySetting { get; private set; }
    
    // Texture and Performance Settings
    public SettingsValue<int> textureQuality { get; private set; }
    public SettingsValue<int> lodBias { get; private set; }
    public SettingsValue<int> targetFramerate { get; private set; }

    private UniversalRenderPipelineAsset customURPAsset;

    public bool CanApplyOrReset() => 
        resolutions.CanApplyOrReset || screenModes.CanApplyOrReset ||
        filmGrain.CanApplyOrReset || bloom.CanApplyOrReset ||
        vignette.CanApplyOrReset || chromaticAberration.CanApplyOrReset ||
        colorGrading.CanApplyOrReset || motionBlur.CanApplyOrReset ||
        renderScale.CanApplyOrReset || msaaQuality.CanApplyOrReset ||
        shadowDistance.CanApplyOrReset || shadowResolution.CanApplyOrReset ||
        qualitySetting.CanApplyOrReset || textureQuality.CanApplyOrReset ||
        lodBias.CanApplyOrReset || vsync.CanApplyOrReset ||
        targetFramerate.CanApplyOrReset;

    public void ApplySettings()
    {
        
        // Apply display settings
        resolutions.Apply();
        screenModes.Apply();
        
        // Apply graphics quality
        qualitySetting.Apply();
        currentQuality = qualitySetting.CurrentValue;
        
        // Apply URP settings
        ApplyURPSettings();
        
        // Apply post-processing
        ApplyVolumeSettings();
        
        // Apply other settings
        vsync.Apply();
        targetFramerate.Apply();
        textureQuality.Apply();
        lodBias.Apply();
    }

    public void ResetSettings()
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
        shadowDistance.Reset();
        shadowResolution.Reset();
        qualitySetting.Reset();
        textureQuality.Reset();
        lodBias.Reset();
        vsync.Reset();
        targetFramerate.Reset();
    }

    public void InitValues()
    {
        // Display Settings
        screenModes = new SettingsValue<FullScreenMode>(
            new[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed },
            SetResolution,
            Screen.fullScreenMode
        );
            
        var availableResolutions = Screen.resolutions.AsValueEnumerable()
            .Where(r => r.refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
            .OrderBy(r => r.width)
            .ToArray();
            
        resolutions = new SettingsValue<Resolution>(
            availableResolutions,
            SetResolution,
            Screen.currentResolution,
            () => resolutions.CurrentValue.width + "x" + resolutions.CurrentValue.height );
        
        // Graphics Quality
        qualitySetting = new SettingsValue<GraphicsQuality>(
            Enum.GetValues(typeof(GraphicsQuality)).AsValueEnumerable().Cast<GraphicsQuality>().ToArray(),
            () => { currentQuality = qualitySetting.CurrentValue; },
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
        renderScale = new SettingsValue<float>(
            new[] { 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 2.0f },
            () => { },
            1.0f
        );
        
        msaaQuality = new SettingsValue<MsaaQuality>(
            Enum.GetValues(typeof(MsaaQuality)).AsValueEnumerable().Cast<MsaaQuality>().ToArray(),
            () => { },
            MsaaQuality.Disabled
        );
        
        shadowDistance = new SettingsValue<float>(
            new[] { 50f, 100f, 150f, 200f, 300f },
            () => { },
            150f
        );
        
        shadowResolution = new SettingsValue<ShadowResolution>(
            Enum.GetValues(typeof(ShadowResolution)).AsValueEnumerable().Cast<ShadowResolution>().ToArray(),
            () => { },
            ShadowResolution._2048
        );
        
        // Performance Settings
        textureQuality = new SettingsValue<int>(
            new[] { 3, 2, 1, 0 }, // 0 = full quality, higher = lower quality
            () => QualitySettings.globalTextureMipmapLimit = textureQuality.CurrentValue,
            0
        );
        
        lodBias = new SettingsValue<int>(
            new[] { 0, 1, 2 }, // LOD bias levels
            () => QualitySettings.lodBias = lodBias.CurrentValue,
            1
        );
        
        vsync = new BoolSetting(
            () => QualitySettings.vSyncCount = vsync.CurrentValue ? 1 : 0,
            true
        );
        
        targetFramerate = new SettingsValue<int>(
            new[] { 30, 60, 120, -1 }, // -1 = unlimited
            () => Application.targetFrameRate = targetFramerate.CurrentValue,
            60
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
        urpAsset.shadowDistance = shadowDistance.CurrentValue;
        urpAsset.mainLightShadowmapResolution = (int)shadowResolution.CurrentValue;
        urpAsset.additionalLightsShadowmapResolution = (int)shadowResolution.CurrentValue;
    }
    
    private void ApplyVolumeSettings()
    {
        if (volumeProfile == null) return;
        
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

public class SettingsValue<T>
{
    private int appliedIndex;
    private int currentIndex;

    private Text label;
    public Text Label
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

    public readonly T[] possibleValues;
    private readonly Action applyAction;
    [CanBeNull] private readonly Func<string> toStringOverride;

    public SettingsValue(T[] possibleValues, Action applyAction, T initialValue = default, Func<string> toStringOverride = null)
    {
        this.possibleValues = possibleValues ?? throw new ArgumentNullException(nameof(possibleValues));
        this.applyAction = applyAction;
        this.toStringOverride = toStringOverride;
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

    public override string ToString() =>
        toStringOverride != null ? toStringOverride.Invoke() : CurrentValue.ToString();
}

public enum GraphicsQuality
{
    Low,
    Medium,
    High
}