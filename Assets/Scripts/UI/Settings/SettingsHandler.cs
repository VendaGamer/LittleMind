using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
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

    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup effectMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;

    // Display Settings
    public ValueSetting<Resolution> resolutions { get; private set; }
    public ValueSetting<FullScreenMode> screenModes { get; private set; }
    
    // Post-Processing Settings
    public BoolSetting filmGrain { get; private set; }
    public BoolSetting bloom { get; private set; }
    public BoolSetting vignette { get; private set; }
    public BoolSetting chromaticAberration { get; private set; }
    public BoolSetting colorGrading { get; private set; }
    public BoolSetting motionBlur { get; private set; }
    public BoolSetting vsync { get; private set; }
    
    // Custom URP Settings
    public ValueSetting<float> renderScale { get; private set; }
    public ValueSetting<MsaaQuality> msaaQuality { get; private set; }
    public ValueSetting<float> shadowDistance { get; private set; }
    public ValueSetting<ShadowResolution> shadowResolution { get; private set; }
    public ValueSetting<GraphicsQuality> Quality { get; private set; }
    
    // Texture and Performance Settings
    public ValueSetting<int> textureQuality { get; private set; }
    public ValueSetting<int> lodBias { get; private set; }
    public ValueSetting<int> targetFramerate { get; private set; }

    private UniversalRenderPipelineAsset customURPAsset;

    public bool CanApplyOrReset() => 
        resolutions.CanApplyOrReset || screenModes.CanApplyOrReset ||
        filmGrain.CanApplyOrReset || bloom.CanApplyOrReset ||
        vignette.CanApplyOrReset || chromaticAberration.CanApplyOrReset ||
        colorGrading.CanApplyOrReset || motionBlur.CanApplyOrReset ||
        renderScale.CanApplyOrReset || msaaQuality.CanApplyOrReset ||
        shadowDistance.CanApplyOrReset || shadowResolution.CanApplyOrReset ||
        Quality.CanApplyOrReset || textureQuality.CanApplyOrReset ||
        lodBias.CanApplyOrReset || vsync.CanApplyOrReset ||
        targetFramerate.CanApplyOrReset;

    public void ApplySettings()
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
        Quality.Reset();
        textureQuality.Reset();
        lodBias.Reset();
        vsync.Reset();
        targetFramerate.Reset();
    }

    public void InitValues()
    {
        // Display Settings
        screenModes = new ValueSetting<FullScreenMode>(
            new[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed },
            SetResolution,
            Screen.fullScreenMode
        );
            
        var availableResolutions = Screen.resolutions.AsValueEnumerable()
            .Where(r => r.refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
            .OrderBy(r => r.width)
            .ToArray();
            
        resolutions = new ValueSetting<Resolution>(
            availableResolutions,
            SetResolution,
            Screen.currentResolution);
        
        // Graphics Quality
        Quality = new ValueSetting<GraphicsQuality>(
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
        renderScale = new ValueSetting<float>(
            new[] { 0.5f, 0.75f, 1.0f, 1.25f, 1.5f, 2.0f },
            () => { },
            1.0f
        );
        
        msaaQuality = new ValueSetting<MsaaQuality>(
            Enum.GetValues(typeof(MsaaQuality)).AsValueEnumerable().Cast<MsaaQuality>().ToArray(),
            () => { },
            MsaaQuality.Disabled
        );
        
        shadowDistance = new ValueSetting<float>(
            new[] { 50f, 100f, 150f, 200f, 300f },
            () => { },
            150f
        );
        
        shadowResolution = new ValueSetting<ShadowResolution>(
            Enum.GetValues(typeof(ShadowResolution)).AsValueEnumerable().Cast<ShadowResolution>().ToArray(),
            () => { },
            ShadowResolution._2048
        );
        
        // Performance Settings
        textureQuality = new ValueSetting<int>(
            new[] { 3, 2, 1, 0 }, // 0 = full quality, higher = lower quality
            () => QualitySettings.globalTextureMipmapLimit = textureQuality.CurrentValue,
            0
        );
        
        lodBias = new ValueSetting<int>(
            new[] { 0, 1, 2 }, // LOD bias levels
            () => QualitySettings.lodBias = lodBias.CurrentValue,
            1
        );
        
        vsync = new BoolSetting(
            () => QualitySettings.vSyncCount = vsync.CurrentValue ? 1 : 0,
            true
        );
        
        targetFramerate = new ValueSetting<int>(
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

public class SliderSetting<T> : ISetting<T> where T : struct
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

    public SliderSetting(Action applyAction, T initialValue = default)
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

public class ValueSetting<T> : ISetting<T> where T : struct
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

    public ValueSetting(T[] possibleValues, Action applyAction, T initialValue = default)
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

public enum GraphicsQuality
{
    Low,
    Medium,
    High
}