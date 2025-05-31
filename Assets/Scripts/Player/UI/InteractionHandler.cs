using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Interactions/Interactions",fileName = "Interactions")]
public class InteractionHandler : ScriptableObject
{
    [SerializeField] private List<UIInteractionGroup> uiInteractionGroups;
    [SerializeField] private bool crosshairVisible = true;
    
    [SerializeField]
    private VisualTreeAsset InteractItemTemplateMouseGamepad;
    [SerializeField]
    private VisualTreeAsset InteractItemTemplateKeyboard;
    [SerializeField]
    private VisualTreeAsset InteractGroupTemplate;
    
    
    private List<GlobalInteractionGroup> _additionalGlobalInteractions = new();
    private List<GlobalInteractionGroup> _currentGlobalInteractions = new();
    private List<IInteractionGroup> _currentInteractableInteractions = new();
    
    private ControlScheme _currentControlScheme;

    public ControlScheme CurrentControlScheme
    {
        get => _currentControlScheme;
        set
        {
            if (value == _currentControlScheme) return;
            
            _currentControlScheme = value;
            Debug.Log($"Changed control scheme to {value}");
            RefreshUI();
        }
    }
    
    private void RefreshUI()
    {
        // Get all global interactions (base + additional)
        var allGlobalInteractions = new List<VisualElement>();
        var allGlobalGroups = new List<GlobalInteractionGroup>();
        allGlobalGroups.AddRange(_currentGlobalInteractions);
        allGlobalGroups.AddRange(_additionalGlobalInteractions);

        foreach (var group in allGlobalGroups)
        {
            var visualGroup = CreateInteractionGroup(group);
            if (visualGroup != null)
                allGlobalInteractions.Add(visualGroup);
        }

        // Get current interactable interactions
        var currentInteractions = new List<VisualElement>();
        foreach (var group in _currentInteractableInteractions)
        {
            var visualGroup = CreateInteractionGroup(group);
            if (visualGroup != null)
                currentInteractions.Add(visualGroup);
        }

        // Update UI
        PlayerUIManager.Instance?.UpdateGlobalInteractions(allGlobalInteractions);
        PlayerUIManager.Instance?.UpdateCurrentInteractions(currentInteractions);
    }
    
    private VisualElement CreateInteractionGroup(IInteractionGroup interactionGroup)
    {
        if (interactionGroup?.CurrentInteractions == null || !interactionGroup.CurrentInteractions.Any())
            return null;

        return CurrentControlScheme switch
        {
            ControlScheme.Gamepad => CreateGamepadGroup(interactionGroup),
            _ => CreateKeyboardMouseGroup(interactionGroup)
        };
    }
    
    
    public void AddAdditionalGlobalInteractions(List<GlobalInteractionGroup> additionalGlobalInteractions)
    {
        foreach (var group in additionalGlobalInteractions)
        {
            if (!_additionalGlobalInteractions.Contains(group))
            {
                _additionalGlobalInteractions.Add(group);
            }
        }
        RefreshUI();
    }

    public void RemoveAdditionalGlobalInteractions(List<GlobalInteractionGroup> additionalGlobalInteractions)
    {
        foreach (var group in additionalGlobalInteractions)
        {
            _additionalGlobalInteractions.Remove(group);
        }
        RefreshUI();
    }

    public void ChangeGlobalInteractions(List<GlobalInteractionGroup> newGlobalInteractions)
    {
        _currentGlobalInteractions.Clear();
        _currentGlobalInteractions.AddRange(newGlobalInteractions);
        RefreshUI();
    }

    public void SetCurrentInteractablesInteractions(IInteractionGroup[] interactions)
    {
        _currentInteractableInteractions.Clear();
        if (interactions != null)
        {
            _currentInteractableInteractions.AddRange(interactions);
        }
        RefreshUI();
    }

    public void OnControlsChanged(PlayerInput playerInput)
    {
        CurrentControlScheme = playerInput.currentControlScheme switch
        {
            "Gamepad" => ControlScheme.Gamepad,
            _ => ControlScheme.KeyboardAndMouse
        };

    }

    private VisualElement CreateGamepadGroup(IInteractionGroup interactionGroup)
    {
        var group = InteractGroupTemplate.Instantiate();
        var listview = group.Q<VisualElement>("Interactions-container");

        foreach (var interaction in interactionGroup.CurrentInteractions)
        {
            var key = XboxGamepadInputPathsNeededForIconFont[interaction.ActionRef.action.activeControl.path];
            var hint = CreateMouseGamepadHint(interaction, key);
            listview.Add(hint);
        }
        return group;
    }
    
    [CanBeNull]
    private VisualElement CreateKeyboardMouseGroup(IInteractionGroup interactionGroup)
    {
        var group = InteractGroupTemplate.Instantiate();
        var container = group.Q<VisualElement>("Interactions-container");

        foreach (var interaction in interactionGroup.CurrentInteractions)
        {
            var devices = interaction.ActionRef.asset.devices;
            if (devices == null) return null;

            var mouseDevice = devices.Value.FirstOrDefault(device => device.valueType == typeof(Mouse));
            
            VisualElement hint;
            if (mouseDevice != null)
            {
                var key = MouseInputPathsNeededForIconFont[interaction.ActionRef.action.activeControl.path];
                hint = CreateMouseGamepadHint(interaction,key);

            }
            else
            {
                var key = interaction.ActionRef.action.activeControl.shortDisplayName;
                hint = CreateKeyboardHint(interaction,key);
            }
            
            container.Add(hint);
        }
        return group;
    }

    private VisualElement CreateMouseGamepadHint(Interaction interaction, string Key)
    {
        var hint = InteractItemTemplateMouseGamepad.Instantiate();
        hint.Q<Label>("key-icon").text = Key;
        hint.Q<Label>("action-name").text = interaction.ActionRef.name;
        return hint;
    }

    private VisualElement CreateKeyboardHint(VisualElement element, Interaction interaction, string Key)
    {
        InteractItemTemplateMouseGamepad.CloneTree(element);
        hint.Q<Label>("key-text").text = Key;
        hint.Q<Label>("action-name").text = interaction.ActionRef.name;
        return hint;
    }
    
    //dict for keys
    
     private Dictionary<string,string> XboxGamepadInputPathsNeededForIconFont= new ()
    {
        {"<Gamepad>/buttonSouth","\u21D3"},
        {"<Gamepad>/buttonWest","\u21D0"},
        {"<Gamepad>/buttonNorth","\u21D1"},
        {"<Gamepad>/buttonEast","\u21D2"},
        {"<Gamepad>/dpad", "\u21CE"},
        {"<Gamepad>/dpad/right", "\u21A0"},
        {"<Gamepad>/dpad/left", "\u219E"},
        {"<Gamepad>/dpad/down", "\u21A1"},
        {"<Gamepad>/dpad/up", "\u219F"},
        {"<Gamepad>/dpad/x", "\u21A2"},
        {"<Gamepad>/dpad/y", "\u21A3"},
        {"<Gamepad>/rightShoulder", "\u2199"},
        {"<Gamepad>/rightStick", "\u21F2"},
        {"<Gamepad>/rightStickPress", "\u21BB"},
        {"<Gamepad>/rightStick/right", "\u21C1"},
        {"<Gamepad>/rightStick/left", "\u21BD"},
        {"<Gamepad>/rightStick/down","\u21C3"},
        {"<Gamepad>/rightStick/up","\u21C3"},
        {"<Gamepad>/rightStick/x","\u21C1"},
        {"<Gamepad>/rightStick/y","\u21C1"},
        {"<Gamepad>/rightTrigger","\u2197"},
        {"<Gamepad>/leftStick", "\u21F1"},
        {"<Gamepad>/leftShoulder", "\u2198"}, 
        {"<Gamepad>/leftStickPress", "\u21BA"},   
        {"<Gamepad>/leftStick/right", "\u21C0"},     
        {"<Gamepad>/leftStick/left", "\u21BC"},      
        {"<Gamepad>/leftStick/down","\u21C2"},       
        {"<Gamepad>/leftStick/up","\u21BE"},
        {"<Gamepad>/leftStick/x","\u21C0"},
        {"<Gamepad>/leftStick/y","\u21C1"},
        {"<Gamepad>/leftTrigger","\u2196"},
        {"<Gamepad>/select", "\u21F7"},
        {"<Gamepad>/start", "\u21F8"},
    };
    
    private Dictionary<string,string> MouseInputPathsNeededForIconFont= new ()
    {
        {"<Mouse>/scroll/up","\u27F0"},
        {"<Mouse>/scroll/down","\u27F1"},
        {"<Mouse>/position","\u27FC"},
        {"<Mouse>/position/x","\u27FA"},
        {"<Mouse>/position/y","\u27FB"},
        {"<Mouse>/leftButton","\u278A"},
        {"<Mouse>/rightButton", "\u278B"},
        {"<Mouse>/middleButton", "\u278C"},
        {"<Mouse>/forwardButton", "\u278D"},
        {"<Mouse>/backButton", "\u278E"},
    };

}

public enum ControlScheme
{
    KeyboardAndMouse,
    Gamepad
}