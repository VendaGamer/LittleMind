using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class VolumeSetting<T> : IAppliableSetting<bool> where T : VolumeComponent
{
    private static readonly string volumeKey = "gfx_volume_" + typeof(T).Name.ToLower();

    private readonly T volumeComponent;
    
    private readonly Toggle toggle;
    
    public bool CurrentValue { get; private set; }
    public bool AppliedValue { get; private set; }
    
    public bool CanApplyOrReset => AppliedValue != CurrentValue;

    public VolumeSetting(Toggle toggle, VolumeProfile volumeProfile)
    {
        toggle.onValueChanged.AddListener(OnToggleValueChanged);
        this.toggle = toggle;
        if (!volumeProfile.TryGet<T>(out var component))
            throw new NullReferenceException("There is no such volume profile component");
        
        volumeComponent = component;
        var initialValue = PlayerPrefs.GetInt(volumeKey, volumeComponent.active ? 1 : 0) is not 0;
        
        volumeComponent.active = initialValue;
        toggle.SetIsOnWithoutNotify(initialValue);
        CurrentValue = initialValue;
        AppliedValue = initialValue;
    }

    private void OnToggleValueChanged(bool value)
    {
        CurrentValue = value;
    }
    public bool Toggle()
    {
        CurrentValue = !CurrentValue;
        return CurrentValue;
    }

    public void Apply()
    {
        if (CanApplyOrReset)
        {
            Debug.Log($"Applied: {typeof(T).Name}");
            AppliedValue = CurrentValue;
            volumeComponent.active = CurrentValue;
            PlayerPrefs.SetInt(volumeKey, AppliedValue ? 1 : 0);
        }
    }

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            CurrentValue = AppliedValue;
            toggle.SetIsOnWithoutNotify(CurrentValue);
        }
    }

    public override string ToString() => CurrentValue ? "Enabled" : "Disabled";
}