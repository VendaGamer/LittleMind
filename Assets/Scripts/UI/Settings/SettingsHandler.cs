using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ZLinq;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

public class SettingsHandler : ScriptableObject
{
    [Header("Quality Presets")]
    [SerializeField] private UniversalRenderPipelineAsset lowQuality, mediumQuality, highQuality;
    
    [Header("Post-Processing")]
    [SerializeField] private VolumeProfile volumeProfile;

    [SerializeField] private AudioMixerGroup masterMixerGroup;
    [SerializeField] private AudioMixerGroup effectMixerGroup;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    
    // Post-Processing Settings
    public VolumeSetting<FilmGrain> FilmGrain { get; private set; }
    public VolumeSetting<Bloom> Bloom { get; private set; }
    public VolumeSetting<Vignette> Vignette { get; private set; }
    public VolumeSetting<ChromaticAberration> ChromaticAberration { get; private set; }
    public BoolSetting MotionBlur { get; private set; }
    public BoolSetting Vsync { get; private set; }
    
    // Custom URP Settings
    public ValueSetting<float> RenderScale { get; private set; }
    public DefinedSetting<MsaaQuality> MSAAQuality { get; private set; }
    public DefinedSetting<float> ShadowDistance { get; private set; }
    public DefinedSetting<ShadowResolution> ShadowResolution { get; private set; }
    public DefinedSetting<GraphicsQuality> Quality { get; private set; }
    
    // Display Settings
    public DefinedSetting<Resolution> Resolution { get; private set; }
    public WindowModeSetting WindowMode { get; private set; }
    
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

    public bool CanApplyOrResetVideoSettings =>
        Resolution.CanApplyOrReset || WindowMode.CanApplyOrReset ||
        Quality.CanApplyOrReset || Vsync.CanApplyOrReset ||
        TargetFramerate.CanApplyOrReset || TextureQuality.CanApplyOrReset ||
        TextureQuality.CanApplyOrReset || LODBias.CanApplyOrReset;

    public void ApplyVideoSettings()
    {
        if (CanApplyOrResetVideoSettings)
        {
            Resolution.Apply();
            WindowMode.Apply();
            Quality.Apply();
            Vsync.Apply();
            TargetFramerate.Apply();
            TextureQuality.Apply();
            LODBias.Apply();
        }
    }

    public void ResetVideoSettings()
    {
        
        Resolution.Reset();
        WindowMode.Reset();
        FilmGrain.Reset();
        Bloom.Reset();
        Vignette.Reset();
        ChromaticAberration.Reset();
        MotionBlur.Reset();
        RenderScale.Reset();
        MSAAQuality.Reset();
        ShadowDistance.Reset();
        ShadowResolution.Reset();
        Quality.Reset();
        TextureQuality.Reset();
        LODBias.Reset();
        Vsync.Reset();
        TargetFramerate.Reset();
    }

    public void InitValues(MenuBase mainMenu, AudioMenu audioMenu, VideoMenu videoMenu)
    {
        // Display Settings
        WindowMode = new WindowModeSetting(
            new[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed },
            SetResolution,
            Screen.fullScreenMode
        );
            
        var availableResolutions = Screen.resolutions
            .AsValueEnumerable()
            .Where(r => r.refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
            .OrderBy(r => r.width)
            .ToArray();
            
        Resolution = new DefinedSetting<Resolution>(
            availableResolutions,
            SetResolution,
            Screen.currentResolution);
        
        // Graphics Quality
        Quality = new DefinedSetting<GraphicsQuality>(
            Enum.GetValues(typeof(GraphicsQuality)).AsValueEnumerable().Cast<GraphicsQuality>().ToArray(),
            () =>
            {
                var targetAsset = GetCurrentURPAsset(Quality.CurrentValue);
                GraphicsSettings.defaultRenderPipeline = targetAsset;
                QualitySettings.renderPipeline = targetAsset;
            },
            GraphicsQuality.High
        );
        

        RenderScale = new ValueSetting<float>(
            () =>
            {
                if (GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset pipeline)
                {
                    pipeline.renderScale = RenderScale.CurrentValue;
                }
            },
            1.0f
        );
        
        MSAAQuality = new DefinedSetting<MsaaQuality>(
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
        
        Vsync = new BoolSetting(
            () => QualitySettings.vSyncCount = Vsync.CurrentValue ? 1 : 0,
            QualitySettings.vSyncCount > 0
        );

        TargetFramerate = new ValueSetting<int>(() =>
            {
                Application.targetFrameRate = TargetFramerate.CurrentValue;
            },
            Application.targetFrameRate
        );

        MasterVolume = new ValueSetting<float>(() =>
        {
            masterMixerGroup.audioMixer.SetFloat(masterMixerGroup.name, MasterVolume.CurrentValue);
        });
        
        EffectsVolume = new ValueSetting<float>(() =>
        {
            effectMixerGroup.audioMixer.SetFloat(effectMixerGroup.name, EffectsVolume.CurrentValue);
        });

        MusicVolume = new ValueSetting<float>(() =>
        {
            musicMixerGroup.audioMixer.SetFloat(musicMixerGroup.name, MusicVolume.CurrentValue);
        });
    }

    private void SetResolution()
    {
        var resolution = Resolution.CurrentValue;
        var screenMode = WindowMode.CurrentValue;
        Screen.SetResolution(resolution.width, resolution.height, screenMode, resolution.refreshRateRatio);
    }
    

    private UniversalRenderPipelineAsset GetCurrentURPAsset(GraphicsQuality currentQuality) =>
        currentQuality switch
        {
            GraphicsQuality.Low => lowQuality,
            GraphicsQuality.Medium => mediumQuality,
            GraphicsQuality.High => highQuality,
            _ => mediumQuality
        };
    
    private void ApplyCustomURPSettings(UniversalRenderPipelineAsset urpAsset)
    {
        urpAsset.renderScale = RenderScale.CurrentValue;
        urpAsset.msaaSampleCount = (int)MSAAQuality.CurrentValue;
        urpAsset.shadowDistance = ShadowDistance.CurrentValue;
        urpAsset.mainLightShadowmapResolution = (int)ShadowResolution.CurrentValue;
        urpAsset.additionalLightsShadowmapResolution = (int)ShadowResolution.CurrentValue;
    }
    
}

public class VolumeSetting<T> : BoolSetting where T : VolumeComponent
{
    private readonly T volumeComponent;
    public VolumeSetting(VolumeProfile volumeProfile, Action applyAction = null, bool initialValue = false) : base(applyAction, initialValue)
    {
        if (volumeProfile.TryGet<T>(out var component))
        {
            volumeComponent = component;
        }
        
        applyAction = () =>
        {
            volumeComponent.active = CurrentValue;
        };
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

    public virtual void Apply()
    {
        if (CanApplyOrReset)
        {
            applyAction.Invoke();
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

public class WindowModeSetting : DefinedSetting<FullScreenMode>
{
    public WindowModeSetting(FullScreenMode[] possibleValues, Action applyAction, FullScreenMode initialValue = default) : base(possibleValues, applyAction, initialValue)
    {
    }

    public override string ToString() =>
        CurrentValue switch
        {
            FullScreenMode.Windowed => "Windowed",
            FullScreenMode.ExclusiveFullScreen => "Exclusive Full Screen",
            FullScreenMode.FullScreenWindow => "Full Screen",
            _ => "Default"
        };
}

public class ValueSetting<T> : ISetting<T> where T : struct
{
    private T currentValue;
    private readonly Action applyAction;

    public T AppliedValue { get; private set; }
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

    public void Apply()
    {
        if (CanApplyOrReset)
        {
            applyAction.Invoke();
            AppliedValue = currentValue;
            label.text = ToString();
        }
    }

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            currentValue = AppliedValue;
            label.text = ToString();
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
            applyAction.Invoke();
            appliedIndex = currentIndex;
        }
    }

    public bool CanApplyOrReset => appliedIndex != currentIndex;

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            currentIndex = appliedIndex;
            label.text = ToString();
        }
    }

    public override string ToString() => CurrentValue.ToString();
}