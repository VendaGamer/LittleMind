using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ValueSetting: IAppliableSetting<float>
{
    private readonly TMP_Text label;
    private readonly Slider slider;
    private readonly Action<float> applyAction;
    private readonly int roundDigits;

    public float AppliedValue { get; private set; }
    public float CurrentValue { get; private set; }
    public bool CanApplyOrReset => !Mathf.Approximately(CurrentValue, AppliedValue);

    public ValueSetting(Slider slider, TMP_Text label, Action<float> applyAction,  float initialValue = 0, int roundDigits = 0)
    {
        slider.onValueChanged.AddListener(OnValueChanged);
        this.slider = slider;
        this.applyAction = applyAction;
        this.roundDigits = roundDigits;
        this.label = label;
        
        CurrentValue = (float)Math.Round(initialValue, roundDigits);
        AppliedValue = CurrentValue;
        label.text = Math.Round(CurrentValue, roundDigits).ToString(CultureInfo.InvariantCulture);
        
        slider.SetValueWithoutNotify(CurrentValue);
        applyAction(CurrentValue);
    }

    private void OnValueChanged(float _)
    {
        CurrentValue = (float)Math.Round(slider.value, roundDigits);
        slider.SetValueWithoutNotify(CurrentValue);
        label.text = CurrentValue.ToString(CultureInfo.InvariantCulture);
    }

    public void Apply()
    {
        if (CanApplyOrReset)
        {
            applyAction.Invoke(CurrentValue);
            AppliedValue = CurrentValue;
            label.text = CurrentValue.ToString(CultureInfo.InvariantCulture);
        }
    }

    public void Reset()
    {
        if (CanApplyOrReset)
        {
            CurrentValue = AppliedValue;
            slider.SetValueWithoutNotify(CurrentValue);
            label.text = CurrentValue.ToString(CultureInfo.InvariantCulture);
        }
    }
}