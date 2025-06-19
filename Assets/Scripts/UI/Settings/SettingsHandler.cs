using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ZLinq;

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
    public VolumeSetting<FilmGrain> FilmGrainSetting { get; private set; }
    public VolumeSetting<Bloom> BloomSetting { get; private set; }
    public VolumeSetting<Vignette> VignetteSetting { get; private set; }
    public VolumeSetting<ChromaticAberration> ChromaticAberrationSetting { get; private set; }
    public VolumeSetting<MotionBlur> MotionBlurSetting { get; private set; }
    public BoolSetting Vsync { get; private set; }
    
    // Custom URP Settings
    public DefinedSetting<GraphicsQuality> QualitySetting { get; private set; }
    
    public ValueSetting<float> RenderScaleSetting { get; private set; }
    
    public DefinedSetting<AntialiasingQuality> AntialiasingQualitySetting { get; private set; }
    public DefinedSetting<AntialiasingMode> AntialiasingModeSetting { get; private set; }
    public DefinedSetting<SettingQuality> ShadowQualitySetting { get; private set; }
    
    // Display Settings
    public DefinedSetting<Resolution> ResolutionSetting { get; private set; }
    public WindowModeSetting WindowModeSetting { get; private set; }
    
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
        ResolutionSetting.CanApplyOrReset || WindowModeSetting.CanApplyOrReset ||
        QualitySetting.CanApplyOrReset || Vsync.CanApplyOrReset ||
        TargetFramerate.CanApplyOrReset || TextureQuality.CanApplyOrReset ||
        TextureQuality.CanApplyOrReset || LODBias.CanApplyOrReset;
    

    public void ApplyVideoSettings()
    {
        if (CanApplyOrResetVideoSettings)
        {
            ResolutionSetting.Apply();
            WindowModeSetting.Apply();
            QualitySetting.Apply();
            Vsync.Apply();
            TargetFramerate.Apply();
            TextureQuality.Apply();
            LODBias.Apply();
        }
    }

    public void ResetVideoSettings()
    {
        
        ResolutionSetting.Reset();
        WindowModeSetting.Reset();
        FilmGrainSetting.Reset();
        BloomSetting.Reset();
        VignetteSetting.Reset();
        ChromaticAberrationSetting.Reset();
        MotionBlurSetting.Reset();
        RenderScaleSetting.Reset();
        ShadowQualitySetting.Reset();
        QualitySetting.Reset();
        TextureQuality.Reset();
        LODBias.Reset();
        Vsync.Reset();
        TargetFramerate.Reset();
    }

    public void InitValues(MenuBase mainMenu, AudioMenu audioMenu, VideoMenu videoMenu)
    {
        // Display Settings
        WindowModeSetting = new WindowModeSetting(
            new[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed },
            () =>
            {
                Screen.fullScreenMode = WindowModeSetting.CurrentValue;
            },
            Screen.fullScreenMode
        );
        
            
        var availableResolutions = Screen.resolutions
            .AsValueEnumerable()
            .Where(r => r.refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
            .OrderBy(r => r.width)
            .ToArray();
            
        ResolutionSetting = new DefinedSetting<Resolution>(
            availableResolutions,
            SetResolution,
            Screen.currentResolution);
        
        
        // Graphics Quality
        QualitySetting = new DefinedSetting<GraphicsQuality>(
            Enum.GetValues(typeof(GraphicsQuality)).AsValueEnumerable().Cast<GraphicsQuality>().ToArray(),
            () =>
            {
                var targetAsset = GetCurrentURPAsset(QualitySetting.CurrentValue);
                QualitySettings.renderPipeline = targetAsset;
                PlayerPrefs.SetInt(GameSettings.VideoSettings.Quality, (int)QualitySetting.CurrentValue);
            },
            (GraphicsQuality)PlayerPrefs.GetInt(
                GameSettings.VideoSettings.Quality,
                (int)GetCurrentGraphicsQuality(QualitySettings.renderPipeline))
        );

        var initialRenderScale = 1.0f;
        
        {
            if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset pipeline)
            {
                initialRenderScale = pipeline.renderScale;
            }
        }

        RenderScaleSetting = new ValueSetting<float>(
            () =>
            {
                if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset pipeline)
                {
                    pipeline.renderScale = RenderScaleSetting.CurrentValue;
                }
            },
            PlayerPrefs.GetFloat(GameSettings.VideoSettings.RenderScale, initialRenderScale)
        );
        
        var urpCameraData = PlayerCamera.Instance.Camera.GetUniversalAdditionalCameraData();
        var initialAntialiasingQuality = urpCameraData.antialiasingQuality;
        var initialAntialiasingMode = urpCameraData.antialiasing;
        
        AntialiasingQualitySetting = new DefinedSetting<AntialiasingQuality>(
            EnumValues.GetAllValues<AntialiasingQuality>(),
            () =>
            {
                var data = PlayerCamera.Instance.Camera.GetUniversalAdditionalCameraData();
                data.antialiasingQuality = AntialiasingQualitySetting.CurrentValue;
            },
            (AntialiasingQuality)PlayerPrefs.GetInt(GameSettings.VideoSettings.AntialiasingQuality, (int)initialAntialiasingQuality)
        );

        AntialiasingModeSetting = new DefinedSetting<AntialiasingMode>(
            EnumValues.GetAllValues<AntialiasingMode>(),
            () =>
            {
                var data = PlayerCamera.Instance.Camera.GetUniversalAdditionalCameraData();
                data.antialiasing = AntialiasingModeSetting.CurrentValue;
            },
            (AntialiasingMode)PlayerPrefs.GetInt(GameSettings.VideoSettings.AntialiasingMode, (int)initialAntialiasingQuality)
        );

        ShadowQualitySetting = new DefinedSetting<SettingQuality>(
            EnumValues.GetAllValues<SettingQuality>(),
            () =>
            {
                
            }
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
        var resolution = ResolutionSetting.CurrentValue;
        var screenMode = WindowModeSetting.CurrentValue;
        Screen.SetResolution(resolution.width, resolution.height, screenMode, resolution.refreshRateRatio);
    }

    private GraphicsQuality GetCurrentGraphicsQuality(RenderPipelineAsset asset)
    {
        if (ReferenceEquals(asset, lowQuality))
        {
            return GraphicsQuality.Low;
        }

        if (ReferenceEquals(asset, mediumQuality))
        {
            return GraphicsQuality.Medium;
        }

        return GraphicsQuality.High;
    }
    

    private UniversalRenderPipelineAsset GetCurrentURPAsset(GraphicsQuality currentQuality) =>
        currentQuality switch
        {
            GraphicsQuality.Low => lowQuality,
            GraphicsQuality.Medium => mediumQuality,
            GraphicsQuality.High => highQuality,
            _ => mediumQuality
        };
    
}