using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class HintManager : MonoBehaviour
{
   public static HintManager Instance { get; private set; }

    [SerializeField] private UIDocument contextMenuDocument;
    [SerializeField] private Font iconFont; // Font containing input icons
    
    private VisualElement menuContainer;
    private Dictionary<Interaction, VisualElement> activeHints = new();
    private Interaction[] currentGlobalInteractions;
    private InputType currentInputType;

    private Dictionary<string,string> XboxGamepadInputPathsNeededForIconFont= new Dictionary<string,string>
    {
        {"<Gamepad>/rightStick/right", "\u21C1"},
        {"<Gamepad>/rightStick/left", "\u21BD"},
        {"<Gamepad>/rightStick/down","\u21C3"},
        {"<Gamepad>/rightStick/up","\u21C3"},
    };

    private void Awake()
    {
        Instance = this;
        InitializeUI();
    }

    private void OnEnable()
    {
        PlayerController.GlobalInteractionsChanged += OnGlobalHintsChanged;
        PlayerController.ExclusiveInteractionsChanged += OnExclusiveHintsChanged;
        PlayerInput.all[0].onControlsChanged += OnControlsChanged;

    }

    private void OnExclusiveHintsChanged(Interaction[] obj)
    {
        
    }

    private void OnGlobalHintsChanged(Interaction[] obj)
    {
        
    }

    private void OnDisable()
    {
        PlayerController.GlobalInteractionsChanged -= OnGlobalHintsChanged;
        PlayerController.ExclusiveInteractionsChanged -= OnExclusiveHintsChanged;
    }

    private void OnControlsChanged(PlayerInput obj)
    {
        
    }

    private string GetDisplayTextForAction(InputActionReference actionRef)
    {
        if (actionRef == null || actionRef.action == null) return string.Empty;

        var binding = actionRef.action.GetBindingForControl(actionRef.action.activeControl);
        if (!binding.HasValue) return string.Empty;
        
        if (currentInputType == InputType.Gamepad)
        {
            return GetGamepadDisplayText(binding.Value);
        }
        else
        {
            return GetKeyboardMouseDisplayText(binding.Value);
        }
    }

    private string GetGamepadDisplayText(InputBinding binding)
    {
        // Map gamepad binding paths to your icon font characters
        // This is just an example - adjust according to your icon font
        return binding.effectivePath switch
        {
            "*/{PrimaryAction}" => "\ue000", // Example: A button icon
            "*/{SecondaryAction}" => "\ue001", // Example: B button icon
            "*/{Submit}" => "\ue002", // Example: X button icon
            // Add more mappings as needed
            _ => binding.ToDisplayString()
        };
    }

    private string GetKeyboardMouseDisplayText(InputBinding binding)
    {
        // For mouse buttons, return icon characters
        if (binding.effectivePath.Contains("Mouse"))
        {
            return binding.effectivePath switch
            {
                "*/leftButton" => "\ue020", // Example: Left mouse icon
                "*/rightButton" => "\ue021", // Example: Right mouse icon
                _ => binding.ToDisplayString()
            };
        }
        
        // For keyboard, return the actual key
        return binding.ToDisplayString().ToUpper();
    }

    private VisualElement CreateHintElement(Interaction interaction)
    {
        var container = new VisualElement();
        container.AddToClassList("menu-item");

        var actionText = new Label(interaction.ActionName);
        actionText.AddToClassList("action-text");

        var keyHint = new Label(GetDisplayTextForAction(interaction.Action));
        keyHint.AddToClassList("key-hint");
        
        // Set the icon font if using icons
        if (currentInputType == InputType.Gamepad || 
            (currentInputType == InputType.KeyboardMouse && IsMouseBinding(interaction.Action)))
        {
            keyHint.style.unityFontDefinition = new StyleFontDefinition(iconFont);
        }

        container.Add(actionText);
        container.Add(keyHint);
        return container;
    }

    private bool IsMouseBinding(InputActionReference actionRef)
    {
        if (actionRef?.action == null) return false;
        var binding = actionRef.action.GetBindingForControl(actionRef.action.activeControl);
        return binding?.effectivePath?.Contains("Mouse") ?? false;
    }


    private void InitializeUI()
    {
        var root = contextMenuDocument.rootVisualElement;
        menuContainer = root.Q<VisualElement>("menu-container");
        menuContainer.style.display = DisplayStyle.None;
    }
    
    private void UpdateInputType(InputType newType)
    {
        if (currentInputType == newType) return;
        
        currentInputType = newType;
    }
    
}