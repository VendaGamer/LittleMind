using UnityEngine.Rendering;

public class VolumeSetting<T> : ISetting<bool> where T : VolumeComponent
{
    private readonly T volumeComponent;
    
    public bool CurrentValue { get; private set; }

    public bool AppliedValue { get; private set; }

    public VolumeSetting(VolumeProfile volumeProfile, bool initialValue = false)
    {
        CurrentValue = initialValue;
        AppliedValue = initialValue;
        if (volumeProfile.TryGet<T>(out var component))
        {
            volumeComponent = component;
        }
    }

    public bool Toggle()
    {
        CurrentValue = !CurrentValue;
        return CurrentValue;
    }

    public void SetValue(bool value)
    {
        CurrentValue = value;
    }

    public void Apply()
    {
        if (CanApplyOrReset)
        {
            volumeComponent.active = CurrentValue;
            AppliedValue = CurrentValue;
        }
    }

    public bool CanApplyOrReset => AppliedValue != CurrentValue;

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            CurrentValue = AppliedValue;
        }
    }

    public override string ToString() => CurrentValue ? "Enabled" : "Disabled";
}