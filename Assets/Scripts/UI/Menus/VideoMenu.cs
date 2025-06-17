using System;
using TMPro;
using UnityEngine;

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