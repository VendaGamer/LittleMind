using System;
using TMPro;
using UnityEngine;

public class WindowModeSetting : DefinedSetting<FullScreenMode>
{
    public WindowModeSetting(TMP_Text label,FullScreenMode[] possibleValues, Action<FullScreenMode> applyAction, FullScreenMode initialValue = default) : base(label,possibleValues, applyAction, initialValue)
    {
    }

    public override string ToString() =>
        CurrentValue switch
        {
            FullScreenMode.Windowed => "Windowed",
            FullScreenMode.ExclusiveFullScreen => "Exclusive FullScreen",
            FullScreenMode.FullScreenWindow => "FullScreen",
            _ => "Default"
        };
}