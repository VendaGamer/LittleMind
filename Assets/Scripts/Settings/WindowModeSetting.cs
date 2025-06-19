using System;
using UnityEngine;

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