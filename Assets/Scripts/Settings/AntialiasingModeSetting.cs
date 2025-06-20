
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class AntialiasingModeSetting : DefinedSetting<AntialiasingMode>
{
    private readonly GameObject qualitySetting;

    public AntialiasingModeSetting(GameObject qualitySettingRootObject, TMP_Text label, AntialiasingMode[] possibleValues, Action<AntialiasingMode> applyAction, AntialiasingMode initialValue = default) : base(label, possibleValues, applyAction, initialValue)
    {
        this.qualitySetting = qualitySettingRootObject;
    }

    public override string ToString()
    {
        switch (CurrentValue)
        {
            case AntialiasingMode.FastApproximateAntialiasing:
                qualitySetting.SetActive(true);
                return "FXAA";
            case AntialiasingMode.SubpixelMorphologicalAntiAliasing:
                qualitySetting.SetActive(true);
                return "SMAA";
            case AntialiasingMode.TemporalAntiAliasing:
                qualitySetting.SetActive(true);
                return "TAA";
            case AntialiasingMode.None:
            default:
                qualitySetting.SetActive(false);
                return "Off";
        }
    }
}
