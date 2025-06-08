using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Interactions/Interactions",fileName = "Interactions")]
public class InteractionHandler : ScriptableObject
{
    [SerializeField]
    private VisualTreeAsset InteractItemTemplateMouseGamepad;
    [SerializeField]
    private VisualTreeAsset InteractItemTemplateKeyboard;

    public Controls InputControls { get; private set; }
    
    private IInteractable currentInteractableInteractionGroup;
    private GlobalInteractionGroup currentGlobalInteractionGroup;

    private string _currentControlSchemeName;
    private string currentControlSchemeName
    {
        get => _currentControlSchemeName;
        set
        {
            if (value == _currentControlSchemeName) return;
            
            _currentControlSchemeName = value;
            RefreshUI();
        }
    }

    private void OnEnable()
    {
        InputControls ??= new Controls();
        currentControlSchemeName ??= InputControls.KeyboardMouseScheme.name;
        InputControls.General.Enable();
    }

    private void OnDisable()
    {
        InputControls.General.Disable();
    }
    
    private void RefreshUI()
    {
        SetGlobalInteractions(currentGlobalInteractionGroup);
        SetCurrentInteractableInteractions(currentInteractableInteractionGroup);
    }
    
    private VisualElement[] CreateInteractionGroupItems(IInteractionGroup interactionGroup)
    {
        return currentControlSchemeName switch
        {
            "Gamepad" => CreateGamepadGroupItems(interactionGroup),
            _ => CreateKeyboardMouseGroupItems(interactionGroup)
        };
    }

    public void SetGlobalInteractions([CanBeNull] GlobalInteractionGroup newGlobalInteractions)
    {
        if (newGlobalInteractions == null)
        {
            PlayerUIManager.Instance.ClearGlobalInteractions();
            return;
        }
        
        currentGlobalInteractionGroup = newGlobalInteractions;
        PlayerUIManager.Instance.GlobalInteractionsLabel.text = newGlobalInteractions.InteractGroupLabel;
        PlayerUIManager.Instance.SetGlobalInteractions(CreateInteractionGroupItems(newGlobalInteractions));
    }

    public void SetCurrentInteractableInteractions([CanBeNull] IInteractable interactions)
    {
        if (currentInteractableInteractionGroup != null)
        {
            currentInteractableInteractionGroup.InteractionsChanged -= OnInteractableInteractionsChanged;
        }
        
        if (interactions == null)
        {
            PlayerUIManager.Instance.ClearCurrentInteractableInteractions();
            return;
        }
        
        currentInteractableInteractionGroup = interactions;
        currentInteractableInteractionGroup.InteractionsChanged += OnInteractableInteractionsChanged;
        PlayerUIManager.Instance.CurrentInteractionsLabel.text = interactions.InteractGroupLabel;
        PlayerUIManager.Instance.SetCurrentInteractions(CreateInteractionGroupItems(interactions));
    }

    private void OnInteractableInteractionsChanged() =>
        PlayerUIManager.Instance.SetCurrentInteractions(CreateInteractionGroupItems(currentInteractableInteractionGroup));

    public void OnControlsChanged(UnityEngine.InputSystem.PlayerInput playerInput)
        => currentControlSchemeName = playerInput.currentControlScheme;

    private VisualElement[] CreateGamepadGroupItems(IInteractionGroup interactionGroup)
    {
        var elements = new VisualElement[interactionGroup.CurrentInteractions.Length];

        for (var i = 0; i < interactionGroup.CurrentInteractions.Length; i++)
        {
            var interaction = interactionGroup.CurrentInteractions[i];
            var action = interaction.ActionRef.action;
            var bindingIndex = action.GetBindingIndex(currentControlSchemeName);

            var binding = action.bindings[bindingIndex];

            var key = EffetivePathToGamepadIcon(binding.effectivePath);
            elements[i] = CreateMouseGamepadHint(interaction, key);
        }

        return elements;
    }
    
    private VisualElement[] CreateKeyboardMouseGroupItems(IInteractionGroup interactionGroup)
    {
        var elements = new VisualElement[interactionGroup.CurrentInteractions.Length];

        for (var i = 0; i < interactionGroup.CurrentInteractions.Length; i++)
        {
            var interaction = interactionGroup.CurrentInteractions[i];
            var action = interaction.ActionRef.action;
            var bindingIndex = action.GetBindingIndex(currentControlSchemeName);

            var binding = action.bindings[bindingIndex];

            if (binding.effectivePath.StartsWith("<Mouse>"))
            {
                var key = EffectivePathToMouseIcon(binding.effectivePath);
                elements[i] = CreateMouseGamepadHint(interaction, key);
            }
            else if (binding.isPartOfComposite)
            {
                elements[i] = CreateMouseGamepadHint(interaction, "\u2423");
            }
            else
            {
                elements[i] = CreateKeyboardHint(interaction, binding.ToDisplayString());
            }
        }

        return elements;
    }

    private VisualElement CreateMouseGamepadHint(Interaction interaction, string key)
    {
        var hint = InteractItemTemplateMouseGamepad.Instantiate();
        hint.Q<Label>("key-icon").text = key;
        hint.Q<Label>("action-name").text = interaction.ActionName;
        return hint;
    }

    private VisualElement CreateKeyboardHint(Interaction interaction, string key)
    {
        var hint = InteractItemTemplateKeyboard.Instantiate();
        hint.Q<Label>("key-text").text = key;
        hint.Q<Label>("action-name").text = interaction.ActionName;
        return hint;
    }
    
    
    private string EffetivePathToGamepadIcon(string effectivePath) =>
        effectivePath switch
        {
            "<Gamepad>/buttonSouth" => "\u21D3",
            "<Gamepad>/buttonWest" => "\u21D0",
            "<Gamepad>/buttonNorth" => "\u21D1",
            "<Gamepad>/buttonEast" => "\u21D2",
            "<Gamepad>/dpad" => "\u21CE",
            "<Gamepad>/dpad/right" => "\u21A0",
            "<Gamepad>/dpad/left" => "\u219E",
            "<Gamepad>/dpad/down" => "\u21A1",
            "<Gamepad>/dpad/up" => "\u219F",
            "<Gamepad>/dpad/x" => "\u21A2",
            "<Gamepad>/dpad/y" => "\u21A3",
            "<Gamepad>/rightShoulder" => "\u2199",
            "<Gamepad>/rightStick" => "\u21F2",
            "<Gamepad>/rightStickPress" => "\u21BB",
            "<Gamepad>/rightStick/right" => "\u21C1",
            "<Gamepad>/rightStick/left" => "\u21BD",
            "<Gamepad>/rightStick/down" => "\u21C3",
            "<Gamepad>/rightStick/up" => "\u21BF",
            "<Gamepad>/rightStick/x" => "\u21C6",
            "<Gamepad>/rightStick/y" => "\u21F5",
            "<Gamepad>/rightTrigger" => "\u2197",
            "<Gamepad>/leftStick" => "\u21F1",
            "<Gamepad>/leftShoulder" => "\u2198", 
            "<Gamepad>/leftStickPress" => "\u21BA", 
            "<Gamepad>/leftStick/right" => "\u21C0",     
            "<Gamepad>/leftStick/left" => "\u21BC",      
            "<Gamepad>/leftStick/down" => "\u21C2",       
            "<Gamepad>/leftStick/up" => "\u21BE",
            "<Gamepad>/leftStick/x" => "\u21C4",
            "<Gamepad>/leftStick/y" => "\u21C5",
            "<Gamepad>/leftTrigger" => "\u2196",
            "<Gamepad>/select" => "\u21F7",
            "<Gamepad>/start" => "\u21F8",
            _ => "?"
        };

    private string EffectivePathToMouseIcon(string effectivePath) =>
        effectivePath switch
        {
            "<Mouse>/scroll/up" => "\u27F0",
            "<Mouse>/scroll/down" => "\u27F1",
            "<Mouse>/delta" => "\u27FC",
            "<Mouse>/position" => "\u27FC",
            "<Mouse>/position/x" => "\u27FA",
            "<Mouse>/position/y" => "\u27FB",
            "<Mouse>/leftButton" => "\u278A",
            "<Mouse>/rightButton" => "\u278B",
            "<Mouse>/middleButton" => "\u278C",
            "<Mouse>/forwardButton" => "\u278D",
            "<Mouse>/backButton" => "\u278E",
            _ => "\u27FC"
        };

}