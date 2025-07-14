using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ZLinq;

public class SettingsHandler : ScriptableObject
{
    public const string masterVolumeKey = "MASTER";
    public const string effectsVolumeKey = "SFX";
    public const string musicVolumeKey = "MUSIC";
    
    
    [Header("Quality Presets")]
    [SerializeField] private UniversalRenderPipelineAsset lowQuality, mediumQuality, highQuality;
    
    [Header("Post-Processing")]
    [SerializeField] private VolumeProfile volumeProfile;

    [SerializeField] private AudioMixer mixer;
    
    // Post-Processing Settings
    public VolumeSetting<FilmGrain> FilmGrainSetting { get; private set; }
    public VolumeSetting<Bloom> BloomSetting { get; private set; }
    public VolumeSetting<Vignette> VignetteSetting { get; private set; }
    public VolumeSetting<ChromaticAberration> ChromaticAberrationSetting { get; private set; }
    public VolumeSetting<MotionBlur> MotionBlurSetting { get; private set; }
    public DefinedSetting<VsyncType> VsyncSetting { get; private set; }
    
    // Custom URP Settings
    public DefinedSetting<GraphicsQuality> QualitySetting { get; private set; }
    public ValueSetting RenderScaleSetting { get; private set; }
    
    public DefinedSetting<AntialiasingQuality> AntialiasingQualitySetting { get; private set; }
    public AntialiasingModeSetting AntialiasingModeSetting { get; private set; }
    
    // Display Settings
    public DefinedSetting<Resolution> ResolutionSetting { get; private set; }
    public WindowModeSetting WindowModeSetting { get; private set; }
    
    // Texture and Performance Settings
    
    public ValueSetting FOVSetting { get; private set; }
    public ValueSetting MasterVolume { get; private set; }
    public ValueSetting MusicVolume { get; private set; }
    public ValueSetting EffectsVolume { get; private set; }
    
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
        QualitySetting.CanApplyOrReset || VsyncSetting.CanApplyOrReset ||
        AntialiasingQualitySetting.CanApplyOrReset || AntialiasingModeSetting.CanApplyOrReset ||
        BloomSetting.CanApplyOrReset || FilmGrainSetting.CanApplyOrReset ||
        ChromaticAberrationSetting.CanApplyOrReset || MotionBlurSetting.CanApplyOrReset ||
        RenderScaleSetting.CanApplyOrReset || VignetteSetting.CanApplyOrReset ||
        FOVSetting.CanApplyOrReset;
    

    public void ApplyVideoSettings()
    {
        if (CanApplyOrResetVideoSettings)
        {
            FOVSetting.Apply();
            ResolutionSetting.Apply();
            WindowModeSetting.Apply();
            FilmGrainSetting.Apply();
            BloomSetting.Apply();
            VignetteSetting.Apply();
            ChromaticAberrationSetting.Apply();
            MotionBlurSetting.Apply();
            RenderScaleSetting.Apply();
            QualitySetting.Apply();
            AntialiasingQualitySetting.Apply();
            AntialiasingModeSetting.Apply();
            VsyncSetting.Apply();
        }
    }

    public void ResetVideoSettings()
    {
        if (CanApplyOrResetVideoSettings)
        {
            FOVSetting.Reset();
            ResolutionSetting.Reset();
            WindowModeSetting.Reset();
            FilmGrainSetting.Reset();
            BloomSetting.Reset();
            VignetteSetting.Reset();
            ChromaticAberrationSetting.Reset();
            MotionBlurSetting.Reset();
            RenderScaleSetting.Reset();
            QualitySetting.Reset();
            AntialiasingQualitySetting.Reset();
            AntialiasingModeSetting.Reset();
            VsyncSetting.Reset();
        }

    }

    public void InitValues(AudioMenu audioMenu, VideoMenu videoMenu)
    {
        // Display Settings
        WindowModeSetting = new WindowModeSetting(
            videoMenu.WindowModeLabel,
            new[] { FullScreenMode.ExclusiveFullScreen, FullScreenMode.FullScreenWindow, FullScreenMode.Windowed },
            static mode =>
            {
                Screen.fullScreenMode = mode;
            },
            Screen.fullScreenMode
        );
        WindowModeSetting.ForceApply();
            
        ResolutionSetting = new DefinedSetting<Resolution>(
            videoMenu.ResolutionLabel,
            Screen.resolutions
                .AsValueEnumerable()
                .Where(r => r.refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
                .OrderBy(r => r.width)
                .ToArray(),
            resolution =>
            {
                Screen.SetResolution(resolution.width, resolution.height,
                    WindowModeSetting.CurrentValue, resolution.refreshRateRatio);
            },
            Screen.currentResolution);
        
        ResolutionSetting.ForceApply();
        
        
        // Graphics Quality
        QualitySetting = new DefinedSetting<GraphicsQuality>(
            videoMenu.PresetLabel,
            Enum.GetValues(typeof(GraphicsQuality)).AsValueEnumerable().Cast<GraphicsQuality>().ToArray(),
            quality =>
            {
                var targetAsset = GetCurrentURPAsset(quality);
                QualitySettings.renderPipeline = targetAsset;
                PlayerPrefs.SetInt(GameSettings.VideoSettings.Quality, (int)quality);
            },
            (GraphicsQuality)PlayerPrefs.GetInt(
                GameSettings.VideoSettings.Quality,
                (int)GetCurrentGraphicsQuality(QualitySettings.renderPipeline))
        );
        QualitySetting.ForceApply();

        RenderScaleSetting = new ValueSetting(videoMenu.RenderScaleSlider, videoMenu.RenderScaleValueLabel,
            static value =>
            {
                if (QualitySettings.renderPipeline is UniversalRenderPipelineAsset pipeline)
                {
                    pipeline.renderScale = value;
                }
            },
            PlayerPrefs.GetFloat(GameSettings.VideoSettings.RenderScale, 1.0f),
            1
        );
        
        var urpCameraData = Camera.main.GetUniversalAdditionalCameraData();
        var initialAntialiasingQuality = urpCameraData.antialiasingQuality;
        var initialAntialiasingMode = urpCameraData.antialiasing;
        
        AntialiasingQualitySetting = new DefinedSetting<AntialiasingQuality>(
            videoMenu.AntialiasingQualityLabel,
            EnumValues.GetAllValues<AntialiasingQuality>(),
            static quality =>
            {
                var data = Camera.main.GetUniversalAdditionalCameraData();
                Debug.Log($"Current quality: {quality}");
                Debug.Log($"Current camera antialiasing quality: {data.antialiasingQuality}");
                data.antialiasingQuality = quality;
                PlayerPrefs.SetInt(GameSettings.VideoSettings.AntialiasingQuality, (int)quality);
            },
            (AntialiasingQuality)PlayerPrefs.GetInt(GameSettings.VideoSettings.AntialiasingQuality,
                (int)initialAntialiasingQuality)
        );
        AntialiasingQualitySetting.ForceApply();

        AntialiasingModeSetting = new AntialiasingModeSetting(
            videoMenu.AntialiasingQualitySettingRoot,
            videoMenu.AntialiasingModeLabel,
            EnumValues.GetAllValues<AntialiasingMode>(),
            static mode =>
            {
                var data = PlayerCamera.Instance.Camera.GetUniversalAdditionalCameraData();
                Debug.Log($"Current mode: {mode}");
                Debug.Log($"Current camera antialiasing mode: {data.antialiasing}");
                data.antialiasing = mode;
                PlayerPrefs.SetInt(GameSettings.VideoSettings.AntialiasingMode, (int)mode);
            },
            (AntialiasingMode)PlayerPrefs.GetInt(GameSettings.VideoSettings.AntialiasingMode, (int)initialAntialiasingMode)
        );
        AntialiasingModeSetting.ForceApply();
        
        FOVSetting = new ValueSetting(videoMenu.FOVSlider,videoMenu.FOVValueLabel,
            static value =>
            {
                PlayerCamera.Instance.PlayerCameraFOV = value;
                PlayerPrefs.SetFloat(GameSettings.VideoSettings.FOV, value);
            },
            PlayerPrefs.GetFloat(GameSettings.VideoSettings.FOV, PlayerCamera.Instance.PlayerCameraFOV));

        VsyncSetting = new DefinedSetting<VsyncType>(
            videoMenu.VsyncLabel,
            new [] { VsyncType.Off ,VsyncType.On, VsyncType.HalfRefreshRate},
            static value =>
            {
                QualitySettings.vSyncCount = (int)value;
                PlayerPrefs.SetInt(GameSettings.VideoSettings.Vsync, (int)value);
            },
            (VsyncType)PlayerPrefs.GetInt(GameSettings.VideoSettings.Vsync, QualitySettings.vSyncCount)
        );
        
        VsyncSetting.ForceApply();

        MasterVolume = new ValueSetting(audioMenu.MasterVolSlider,audioMenu.MasterVolValueLabel, 
        value =>
        {
            mixer.SetFloat(masterVolumeKey, SliderValueToDB(value));
            PlayerPrefs.SetFloat(GameSettings.AudioSettings.MasterVolume, value);
        },
        PlayerPrefs.GetFloat(GameSettings.AudioSettings.MasterVolume, 100f));
        
        EffectsVolume = new ValueSetting(audioMenu.EffectVolSlider,audioMenu.EffectVolValueLabel, 
        value =>
        {
            mixer.SetFloat(effectsVolumeKey, SliderValueToDB(value));
            PlayerPrefs.SetFloat(GameSettings.AudioSettings.EffectsVolume, value);
        },
        PlayerPrefs.GetFloat(GameSettings.AudioSettings.EffectsVolume, 100f));

        MusicVolume = new ValueSetting(audioMenu.MusicVolSlider,audioMenu.MusicVolValueLabel,
        value =>
        {
            mixer.SetFloat(musicVolumeKey, SliderValueToDB(value));
            PlayerPrefs.SetFloat(GameSettings.AudioSettings.MusicVolume, value);
        },
        PlayerPrefs.GetFloat(GameSettings.AudioSettings.MusicVolume, 100f));

        BloomSetting = new VolumeSetting<Bloom>(videoMenu.BloomToggle,volumeProfile);
        VignetteSetting = new VolumeSetting<Vignette>(videoMenu.VignetteToggle,volumeProfile);
        ChromaticAberrationSetting = new VolumeSetting<ChromaticAberration>(videoMenu.ChromaticAberrationToggle,volumeProfile);
        FilmGrainSetting = new VolumeSetting<FilmGrain>(videoMenu.FilmGrainToggle,volumeProfile);
        MotionBlurSetting = new VolumeSetting<MotionBlur>(videoMenu.MotionBlurToggle,volumeProfile);
    }

    private static float SliderValueToDB(float sliderValue) => 20.0f * Mathf.Log10(sliderValue / 100f);

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