using System;
using UnityEngine.UI;

public class BoolSetting : IAppliableSetting<bool>
{
    private readonly Toggle toggle;
    private readonly Action<bool> applyAction;

    public bool CurrentValue { get; private set; }

    public bool AppliedValue { get; private set; }

    public BoolSetting(Toggle toggle,Action<bool> applyAction, bool initialValue = false)
    {
        this.toggle = toggle;
        this.applyAction = applyAction;
        CurrentValue = initialValue;
        AppliedValue = initialValue;
        toggle.isOn = CurrentValue;
        applyAction(CurrentValue);
    }

    public void Toggle()
    {
        CurrentValue = !CurrentValue;
        toggle.isOn = CurrentValue;
    }

    public void SetValue(bool value)
    {
        CurrentValue = value;
        toggle.isOn = CurrentValue;
    }

    public void Apply()
    {
        if (CanApplyOrReset)
        {
            applyAction.Invoke(CurrentValue);
            AppliedValue = CurrentValue;
            toggle.isOn = CurrentValue;
        }
    }

    public bool CanApplyOrReset => AppliedValue != CurrentValue;

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            CurrentValue = AppliedValue;
            toggle.isOn = CurrentValue;
        }
    }

    public override string ToString() => CurrentValue ? "Enabled" : "Disabled";
}