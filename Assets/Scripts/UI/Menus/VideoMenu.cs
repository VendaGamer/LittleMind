using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class VideoMenu : MenuBase
{
    public TMP_Text PresetLabel, ResolutionLabel, WindowModeLabel, FOVValueLabel, RenderScaleValueLabel,
        AntialiasingModeLabel, AntialiasingQualityLabel, VsyncLabel;
    
    public Toggle FilmGrainToggle, MotionBlurToggle, VignetteToggle, BloomToggle, ChromaticAberrationToggle;
    
    public Slider FOVSlider, RenderScaleSlider;
    
    public GameObject AntialiasingQualitySettingRoot;
}