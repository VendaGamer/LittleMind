using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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