using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class HintData
{
    public string ActionName;
    public Sprite Icon; // Icon to display for controllers
    public string KeyHint; // Text to display for keyboard
    public InputActionReference ActionReference;
    public bool IsGlobal; // Indicates if it's a global action

    public HintData(string actionName, Sprite icon, string keyHint, InputActionReference actionReference, bool isGlobal = false)
    {
        ActionName = actionName;
        Icon = icon;
        KeyHint = keyHint;
        ActionReference = actionReference;
        IsGlobal = isGlobal;
    }
}