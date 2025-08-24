using System;
using TMPro;

public class DefinedSetting<T> : IAppliableSetting<T> where T : struct
{
    private int appliedIndex;
    private int currentIndex;

    private TMP_Text label;
    public T CurrentValue => possibleValues[currentIndex];
    public T AppliedValue => possibleValues[appliedIndex];
    private readonly T[] possibleValues;
    private readonly Action<T> applyAction;

    public DefinedSetting(TMP_Text label, T[] possibleValues, Action<T> applyAction, T initialValue = default)
    {
        this.label = label;
        this.possibleValues = possibleValues;
        this.applyAction = applyAction;
        
        currentIndex = Array.IndexOf(possibleValues, initialValue);
        appliedIndex = currentIndex;
    }

    public virtual void ForceApply()
    {
        try
        {
            applyAction.Invoke(CurrentValue);
            appliedIndex = currentIndex;
            label.text = ToString();
        }
        catch
        {
            // ignored
        }
    }

    public T NextValue()
    {
        currentIndex++;
        if (currentIndex > possibleValues.Length - 1)
        {
            currentIndex = 0;
        }

        label.text = ToString();
        return CurrentValue;
    }

    public T PreviousValue()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = possibleValues.Length - 1;
        }
        label.text = ToString();
        return CurrentValue;
    }

    public void SetCurrentIndex(int index)
    {
        if (index >= 0 && index < possibleValues.Length)
            currentIndex = index;
    }

    public void Apply()
    {
        if (CanApplyOrReset)
        {
            applyAction.Invoke(CurrentValue);
            appliedIndex = currentIndex;
            label.text = ToString();
        }
    }

    public bool CanApplyOrReset => appliedIndex != currentIndex;

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            currentIndex = appliedIndex;
            label.text = ToString();
        }
    }

    public override string ToString() => CurrentValue.ToString();
}