using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using ZLinq;
using ShadowResolution = UnityEngine.Rendering.Universal.ShadowResolution;

[CreateAssetMenu(menuName = "GameSettings/SettingsHandler", fileName = "SettingsHandler")]
public class SettingsHandler : ScriptableObject
{
    [SerializeField]
    private UniversalRenderPipelineAsset lowQuality, mediumQuality, highQuality;
    
    [Header("Post-Processing")]
    [SerializeField] private VolumeProfile volumeProfile;
    
    [Header("Custom Settings")]
    public GraphicsQuality currentQuality = GraphicsQuality.High;

    public SettingsValue<Resolution> resolutions { get; private set; }
    public SettingsValue<FullScreenMode> screenModes  { get; private set; }
    public bool filmGrainEnabled = true;
    public bool bloomEnabled = true;
    public bool vignetteEnabled = true;
    public bool chromaticAberrationEnabled = true;
    
    // Custom URP settings
    [Header("Custom URP Settings")]
    [Range(0.5f, 2.0f)] public float renderScale = 1.0f;
    public MsaaQuality msaaQuality = MsaaQuality.Disabled;
    [Range(50f, 300f)] public float shadowDistance = 150f;
    public ShadowResolution shadowResolution = ShadowResolution._2048;
    
    private UniversalRenderPipelineAsset customURPAsset;


    private void SetResolution()
    {
        Screen.SetResolution(resolutions.CurrentValue.width, resolutions.CurrentValue.height,
        screenModes.CurrentValue, resolutions.CurrentValue.refreshRateRatio);
    }
    private void OnEnable()
    {
        screenModes = new SettingsValue<FullScreenMode>(
            new[]
            {
                FullScreenMode.ExclusiveFullScreen,
                FullScreenMode.FullScreenWindow,
                FullScreenMode.Windowed,
            },
            SetResolution
        );
            
        resolutions = new SettingsValue<Resolution>(
            Screen.resolutions.AsValueEnumerable()
                .Where(r => r.refreshRateRatio.Equals(Screen.currentResolution.refreshRateRatio))
                .Order()
                .ToArray(),
            SetResolution
            ,
            Screen.currentResolution
            );
        

        
    }
    
    private UniversalRenderPipelineAsset GetOrCreateCustomURPAsset()
    {
        if (customURPAsset) 
            return customURPAsset;
        
        customURPAsset = Instantiate((UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline);
        customURPAsset.name = "Custom_URP_Asset";
        return customURPAsset;
    }

    public void ApplySettings()
    {

    }

    private UniversalRenderPipelineAsset GetCurrentURPAsset() =>
        currentQuality switch
        {
            GraphicsQuality.Low => lowQuality,
            GraphicsQuality.Medium => mediumQuality,
            GraphicsQuality.High => highQuality,
            _ => GetOrCreateCustomURPAsset()
        };
    
    private void ApplyURPSettings()
    {
        var targetAsset = GetCurrentURPAsset();
        
        GraphicsSettings.defaultRenderPipeline = targetAsset;
        QualitySettings.renderPipeline = targetAsset;
        
        // If using custom settings, modify the asset
        if (currentQuality == GraphicsQuality.Custom)
        {
            ApplyCustomURPSettings(targetAsset);
        }
    }
    private void ApplyCustomURPSettings(UniversalRenderPipelineAsset urpAsset)
    {
        // Modify URP asset properties
        urpAsset.renderScale = renderScale;
        urpAsset.msaaSampleCount = (int)msaaQuality;
        urpAsset.shadowDistance = shadowDistance;
        urpAsset.mainLightShadowmapResolution = (int)shadowResolution;
        urpAsset.additionalLightsShadowmapResolution = (int)shadowResolution;
    }
    
    private void Reset()
    {

    }
    
}

public class SettingsValue<T> where T : struct
{
    private int appliedIndex;
    private int currentIndex;

    public T CurrentValue => possibleValues[currentIndex];

    public readonly T[] possibleValues;
    private readonly Action applyAction;

    public SettingsValue(T[] possibleValues, Action applyAction, T initialValue = default)
    {
        this.possibleValues = possibleValues;
        this.applyAction = applyAction;
        currentIndex = Array.IndexOf(possibleValues, initialValue);
        appliedIndex = currentIndex;
    }

    public T NextValue()
    {
        currentIndex++;
        if (currentIndex > possibleValues.Length)
        {
            currentIndex = 0;
        }
        return possibleValues[currentIndex];
    }

    public T PreviousValue()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = possibleValues.Length - 1;
        }
        return possibleValues[currentIndex];
    }

    public bool Apply()
    {
        if(appliedIndex == currentIndex)
            return false;
        
        applyAction?.Invoke();
        appliedIndex = currentIndex;
        return true;
    }

    public bool CanApplyOrReset => appliedIndex != currentIndex;

    public void Reset()
    {
        
        currentIndex = appliedIndex;
    }
    
    public override string ToString() => CurrentValue.ToString();
}

public enum GraphicsQuality
{
    Low,
    Medium,
    High,
    Custom
}

public enum GraphicsItemQuality
{
    Low,
    Medium,
    High,
    Ultra
}

public class GraphicsItem
{
    private GraphicsItemQuality quality;
    private GraphicsItemQuality[] qualities;
}