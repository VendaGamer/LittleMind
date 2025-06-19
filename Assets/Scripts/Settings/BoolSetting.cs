using System;

public class BoolSetting : ISetting<bool>
{
    private readonly Action applyAction;

    public bool CurrentValue { get; private set; }

    public bool AppliedValue { get; private set; }

    public BoolSetting(Action applyAction, bool initialValue = false)
    {
        this.applyAction = applyAction;
        CurrentValue = initialValue;
        AppliedValue = initialValue;
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
            applyAction.Invoke();
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