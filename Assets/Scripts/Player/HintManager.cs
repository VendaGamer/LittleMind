using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class HintManager : MonoBehaviour
{
    private UIDocument document;
    private VisualElement menuContainer;

    // Store active hints in a HashSet for efficient lookups
    private HashSet<string> activeHintKeys = new HashSet<string>();
    private Dictionary<string, VisualElement> hintElements = new Dictionary<string, VisualElement>();

    private InputDevice currentDevice;

    private void OnEnable()
    {
        document = GetComponent<UIDocument>();
        menuContainer = document.rootVisualElement.Q("menu-container");

        InputSystem.onDeviceChange += OnDeviceChange;
        UpdateCurrentDevice();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.UsageChanged || change == InputDeviceChange.Added)
        {
            UpdateCurrentDevice();
        }
    }

    private void UpdateCurrentDevice()
    {
        currentDevice = InputSystem.GetDevice<Keyboard>() ?? InputSystem.GetDevice<Gamepad>() as InputDevice;
        RefreshAllHints();
    }

    public void AddHint(HintData hint)
    {
        if (activeHintKeys.Contains(hint.ActionName)) return;

        activeHintKeys.Add(hint.ActionName);
        var hintElement = CreateHintElement(hint);
        menuContainer.Add(hintElement);
        hintElements[hint.ActionName] = hintElement;
    }

    public void RemoveHint(string actionName)
    {
        if (!activeHintKeys.Contains(actionName)) return;

        activeHintKeys.Remove(actionName);

        if (hintElements.TryGetValue(actionName, out var element))
        {
            menuContainer.Remove(element);
            hintElements.Remove(actionName);
        }
    }

    public void RefreshAllHints()
    {
        foreach (var key in activeHintKeys)
        {
            if (hintElements.TryGetValue(key, out var element))
            {
                menuContainer.Remove(element);
            }
        }

        hintElements.Clear();
        activeHintKeys.Clear();
    }

    private VisualElement CreateHintElement(HintData hint)
    {
        var hintElement = new VisualElement();
        hintElement.AddToClassList("menu-item");

        if (currentDevice is Gamepad && hint.Icon)
        {
            var icon = new Image { sprite = hint.Icon };
            icon.AddToClassList("key-hint");
            hintElement.Add(icon);
        }
        else
        {
            var keyHint = new Label(hint.KeyHint);
            keyHint.AddToClassList("key-hint");
            hintElement.Add(keyHint);
        }

        var actionText = new Label(hint.ActionName);
        actionText.AddToClassList("action-text");
        hintElement.Add(actionText);

        return hintElement;
    }
}
