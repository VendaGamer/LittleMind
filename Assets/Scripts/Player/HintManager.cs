using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    [SerializeField] private UIDocument contextMenuDocument;
    
    private VisualElement menuContainer;
    private Dictionary<IInteractions, VisualElement> interactionsAndTheirContainer = new();
    
    private InputType currentInputType = InputType.KeyboardMouse;
    private VisualElement crosshair;

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


    private void Awake()
    {
        Instance = this;
        InitializeUI();
    }

    private void OnEnable()
    {
        PlayerController.GlobalInteractionsChanged += OnGlobalHintsChanged;
        PlayerController.ExclusiveInteractableChanged += OnExclusiveHintsChanged;
        PlayerInput.all[0].onControlsChanged += OnControlsChanged;
    }

    private void OnExclusiveHintsChanged([CanBeNull]IInteractable obj)
    {
        // Find any existing exclusive interactions

        if (interactionsAndTheirContainer.Keys
                .FirstOrDefault(x => x is IInteractable) is IInteractable existingExclusive)
        {
            DeleteExclusiveInteractions();
        }

        if (obj == null)
        {
            crosshair.RemoveFromClassList("crosshair--interactive");
            return;
        }

        var container = CreateElements(obj);
        interactionsAndTheirContainer[obj] = container;
        menuContainer.Add(container);
        menuContainer.style.display = DisplayStyle.Flex;
        crosshair.AddToClassList("crosshair--interactive");
        if (!interactionsAndTheirContainer.Any())
        {
            menuContainer.style.display = DisplayStyle.None;
            crosshair.RemoveFromClassList("crosshair--interactive");
        }
    }

    private void OnGlobalHintsChanged(GlobalInteractions obj)
    {
        if (interactionsAndTheirContainer.Keys
                .FirstOrDefault(x => x is GlobalInteractions) is GlobalInteractions existingGlobal)
        {
            DeleteGlobalInteractions();
        }
        
        var container = CreateElements(obj);
        interactionsAndTheirContainer[obj] = container;
        menuContainer.Add(container);
        menuContainer.style.display = DisplayStyle.Flex;
            
        if (!interactionsAndTheirContainer.Any())
        {
            menuContainer.style.display = DisplayStyle.None;
        }
    }

    private void OnDisable()
    {
        PlayerController.GlobalInteractionsChanged -= OnGlobalHintsChanged;
        PlayerController.ExclusiveInteractableChanged -= OnExclusiveHintsChanged;
    }

    private void OnControlsChanged(PlayerInput obj)
    {
        var newInputType = obj.currentControlScheme.Contains("Gamepad") 
            ? InputType.Gamepad 
            : InputType.KeyboardMouse;
        
        if(currentInputType==newInputType)return;
        
        currentInputType = newInputType;
            

        foreach (var container in interactionsAndTheirContainer.Values)
        {
            menuContainer.Remove(container);
        }

        var tempDict = new Dictionary<IInteractions, VisualElement>(interactionsAndTheirContainer);
        interactionsAndTheirContainer.Clear();
        
        // Musim znovu vytvorit elementy
        foreach (var kvp in tempDict)
        {
            var newContainer = CreateElements(kvp.Key);
            
            interactionsAndTheirContainer[kvp.Key] = newContainer;
            menuContainer.Add(newContainer);
        }
    }

    
    private (bool isIcon,string text) GetDisplayTextForAction(InputActionReference actionRef)
    {
        if (currentInputType == InputType.Gamepad)
        {
            var binding = actionRef.action.bindings
                .FirstOrDefault(inputBinding => inputBinding.effectivePath.Contains("<Gamepad>"));
            
            if (XboxGamepadInputPathsNeededForIconFont.TryGetValue(binding.effectivePath, out string icon))
            {
                return (true,icon);
            }
            
            return (false,binding.name);
        }
        else // KeyboardMouse
        {
            var binding = actionRef.action.bindings
                .FirstOrDefault(inputBinding => inputBinding.effectivePath.Contains("<Keyboard>")||
                                                inputBinding.effectivePath.Contains("<Mouse>"));
            if (MouseInputPathsNeededForIconFont.TryGetValue(binding.effectivePath, out string icon))
            {
                return (true,icon);
            }
            
            return (false,actionRef.action.GetBindingDisplayString());
        }
    }

    private void DeleteGlobalInteractions()
    {
        var globalInteractions = interactionsAndTheirContainer.Keys
            .FirstOrDefault(x => x is GlobalInteractions);
    
        if (globalInteractions != null)
        {
            var container = interactionsAndTheirContainer[globalInteractions];
            menuContainer.Remove(container);
            interactionsAndTheirContainer.Remove(globalInteractions);
        }
    }

    private void DeleteExclusiveInteractions()
    {
        var exclusiveInteractions = interactionsAndTheirContainer.Keys
            .FirstOrDefault(x => x is IInteractable);
    
        if (exclusiveInteractions != null)
        {
            var container = interactionsAndTheirContainer[exclusiveInteractions];
            menuContainer.Remove(container);
            interactionsAndTheirContainer.Remove(exclusiveInteractions);
        }
    }

    private VisualElement CreateElements(IInteractions interactable)
    {
        var groupContainer = new VisualElement();
        groupContainer.AddToClassList("interactions-group");
        var groupLabel = new Label(interactable.InteractGroupLabel);
        groupLabel.AddToClassList("interactions-label");
        groupContainer.Add(groupLabel);
        var interactionsContainer = new VisualElement();
        foreach (var interaction in interactable.CurrentInteractions)
        {
            var container = new VisualElement();
            container.AddToClassList("menu-item");
            var actionText = new Label(interaction.ActionName);
            actionText.AddToClassList("action-text");
            var res = GetDisplayTextForAction(interaction.Action);
            var keyHint = new Label(res.text);
            if (res.isIcon)
            {
                keyHint.AddToClassList("key-hint-icon");
                container.Add(actionText);
                container.Add(keyHint);
            }
            else
            {
                var keyContainer = new VisualElement();
                keyContainer.AddToClassList("keyboard-key");
                keyHint.AddToClassList("key-hint-text");
                keyContainer.Add(keyHint);
                container.Add(actionText);
                container.Add(keyContainer);
            }
            interactionsContainer.Add(container);
        }
    
        groupContainer.Add(interactionsContainer);
        return groupContainer;
    }


    private void InitializeUI()
    {
        var root = contextMenuDocument.rootVisualElement;
        menuContainer = root.Q<VisualElement>("menu-container");
        menuContainer.style.display = DisplayStyle.None;
        crosshair = root.Q<VisualElement>("crosshair");
    }
    
}