using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIInteractionsData : MonoBehaviour
{
    public List<UIInteractionGroup> Groups { get; private set; } = new();
    public bool HasInteractableCrosshair { get; private set; }

    public void UpdateInteractions(IInteractions globalInteractions, IInteractable exclusiveInteractable)
    {
        Groups.Clear();

        // Add global interactions
        if (globalInteractions != null)
        {
            Groups.Add(CreateGroupFromInteractions(globalInteractions));
        }

        // Add exclusive interactions
        if (exclusiveInteractable != null)
        {
            Groups.Add(CreateGroupFromInteractions(exclusiveInteractable));
            HasInteractableCrosshair = true;
        }
        else
        {
            HasInteractableCrosshair = false;
        }

        // Notify UI to refresh
        OnInteractionsChanged?.Invoke();
    }

    private UIInteractionGroup CreateGroupFromInteractions(IInteractions interactions)
    {
        var group = new UIInteractionGroup
        {
            GroupLabel = interactions.InteractGroupLabel
        };

        foreach (var interaction in interactions.CurrentInteractions)
        {
            var displayInfo = GetDisplayTextForAction(interaction.ActionRef);

            group.Interactions.Add(new UIInteractionItem
            {
                ActionName = interaction.ActionName,
                DisplayText = displayInfo.text,
                IsIcon = displayInfo.isIcon
            });
        }

        return group;
    }

    private (bool isIcon, string text) GetDisplayTextForAction(InputActionReference actionRef)
    {
        bool isGamepad = PlayerInput.all[0].currentControlScheme.Contains("Gamepad");

        if (isGamepad)
        {
            var binding = actionRef.action.bindings
                .FirstOrDefault(inputBinding => inputBinding.effectivePath.Contains("<Gamepad>"));

            if (!string.IsNullOrEmpty(binding.effectivePath) &&
                xboxGamepadInputPathsNeededForIconFont.TryGetValue(binding.effectivePath, out string icon))
            {
                return (true, icon);
            }
        }
        else // KeyboardMouse
        {
            var binding = actionRef.action.bindings
                .FirstOrDefault(inputBinding => inputBinding.effectivePath.Contains("<Keyboard>") ||
                                                inputBinding.effectivePath.Contains("<Mouse>"));

            if (!string.IsNullOrEmpty(binding.effectivePath) &&
                mouseInputPathsNeededForIconFont.TryGetValue(binding.effectivePath, out string icon))
            {
                return (true, icon);
            }
        }

        return (false, actionRef.action.GetBindingDisplayString(
            InputBinding.DisplayStringOptions.IgnoreBindingOverrides));
    }

    private readonly Dictionary<string, string> xboxGamepadInputPathsNeededForIconFont = new()
    {
        { "<Gamepad>/buttonSouth", "\u21D3" },
        { "<Gamepad>/buttonWest", "\u21D0" },
        { "<Gamepad>/buttonNorth", "\u21D1" },
        { "<Gamepad>/buttonEast", "\u21D2" },
        { "<Gamepad>/dpad", "\u21CE" },
        { "<Gamepad>/dpad/right", "\u21A0" },
        { "<Gamepad>/dpad/left", "\u219E" },
        { "<Gamepad>/dpad/down", "\u21A1" },
        { "<Gamepad>/dpad/up", "\u219F" },
        { "<Gamepad>/dpad/x", "\u21A2" },
        { "<Gamepad>/dpad/y", "\u21A3" },
        { "<Gamepad>/rightShoulder", "\u2199" },
        { "<Gamepad>/rightStick", "\u21F2" },
        { "<Gamepad>/rightStickPress", "\u21BB" },
        { "<Gamepad>/rightStick/right", "\u21C1" },
        { "<Gamepad>/rightStick/left", "\u21BD" },
        { "<Gamepad>/rightStick/down", "\u21C3" },
        { "<Gamepad>/rightStick/up", "\u21C3" },
        { "<Gamepad>/rightStick/x", "\u21C1" },
        { "<Gamepad>/rightStick/y", "\u21C1" },
        { "<Gamepad>/rightTrigger", "\u2197" },
        { "<Gamepad>/leftStick", "\u21F1" },
        { "<Gamepad>/leftShoulder", "\u2198" },
        { "<Gamepad>/leftStickPress", "\u21BA" },
        { "<Gamepad>/leftStick/right", "\u21C0" },
        { "<Gamepad>/leftStick/left", "\u21BC" },
        { "<Gamepad>/leftStick/down", "\u21C2" },
        { "<Gamepad>/leftStick/up", "\u21BE" },
        { "<Gamepad>/leftStick/x", "\u21C0" },
        { "<Gamepad>/leftStick/y", "\u21C1" },
        { "<Gamepad>/leftTrigger", "\u2196" },
        { "<Gamepad>/select", "\u21F7" },
        { "<Gamepad>/start", "\u21F8" },
    };



    private readonly Dictionary<string, string> mouseInputPathsNeededForIconFont = new()
    {
        { "<Mouse>/scroll/up", "\u27F0" },
        { "<Mouse>/scroll/down", "\u27F1" },
        { "<Mouse>/position", "\u27FC" },
        { "<Mouse>/position/x", "\u27FA" },
        { "<Mouse>/position/y", "\u27FB" },
        { "<Mouse>/leftButton", "\u278A" },
        { "<Mouse>/rightButton", "\u278B" },
        { "<Mouse>/middleButton", "\u278C" },
        { "<Mouse>/forwardButton", "\u278D" },
        { "<Mouse>/backButton", "\u278E" },
    };

    // Event for UI to subscribe to
    public event Action OnInteractionsChanged;
}