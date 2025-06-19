using System;
using TMPro;

public class ValueSetting<T> : ISetting<T> where T : struct
{
    private T currentValue;
    private readonly Action applyAction;

    public T AppliedValue { get; private set; }
    public bool CanApplyOrReset => !currentValue.Equals(AppliedValue);

    public T CurrentValue => currentValue;

    public void SetValue(T value)
    {
        if (!currentValue.Equals(value))
        {
            label.text = value.ToString();
        }
    }

    private TMP_Text label;
    
    public TMP_Text Label
    {
        get => label;
        set
        {
            label = value;
            label.text = ToString();
        }
    }

    public ValueSetting(Action applyAction, T initialValue = default)
    {
        this.applyAction = applyAction;
        currentValue = initialValue;
        AppliedValue = currentValue;
    }

    public void Apply()
    {
        if (CanApplyOrReset)
        {
            applyAction.Invoke();
            AppliedValue = currentValue;
            label.text = ToString();
        }
    }

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            currentValue = AppliedValue;
            label.text = ToString();
        }
    }
}