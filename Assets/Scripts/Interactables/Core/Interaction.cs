using System;
using System.Runtime.CompilerServices;
using Unity.Properties;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


[GeneratePropertyBag]
[Serializable]
public partial class Interaction : INotifyBindablePropertyChanged, IDataSourceViewHashProvider
{
    [SerializeField, DontCreateProperty]
    private string actionName;

    [FormerlySerializedAs("Action")]
    [SerializeField, DontCreateProperty]
    private InputActionReference actionRef;

    [CreateProperty]
    public InputAction Action => actionRef.action;
    
    [CreateProperty]
    public string ActionName => actionName;
    
    private string key;
    private bool isIcon;

    [CreateProperty]
    public bool IsIcon
    {
        get => isIcon;
        private set
        {
            if(value == isIcon)
                return;
            
            isIcon = value;
            Notify();
        }
    }
    
    [CreateProperty]
    public string Key
    {
        get
        {
            if (key == null)
            {
                RebuildKey();
            }
            return key;
        }
        private set
        {
            if(value == key)
                return;
            
            key = value;
            Notify();
        }
    }
        
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;


    /// <summary>
    /// Should be called when changing control schemes.
    /// </summary>

    private const string WASDIcon = "\u2423";
    public void RebuildKey()
    {
        var bindingIndex = Action.GetBindingIndex(InputManager.CurrentControlScheme);
        var binding = Action.bindings[bindingIndex];
        
        if (binding.effectivePath.StartsWith("<Keyboard>"))
        {
            if (binding.isPartOfComposite)
            {
                Key = WASDIcon;
                IsIcon = true;
            }
            else
            {
                Key = binding.ToDisplayString();
                IsIcon = false;
            }
        }
        else if (binding.effectivePath.StartsWith("<Gamepad>"))
        {
            Key = EffetivePathToGamepadIcon(binding.effectivePath);
            IsIcon = true;
        }
        else if (binding.effectivePath.StartsWith("<Mouse>"))
        {
            Key = EffectivePathToMouseIcon(binding.effectivePath);
            IsIcon = true;
        }
        else
        {
            Key = binding.ToDisplayString();
            IsIcon = false;
        }
        
    }
    
    private static string EffetivePathToGamepadIcon(string effectivePath) =>
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

    private static string EffectivePathToMouseIcon(string effectivePath) =>
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
    
    private void Notify([CallerMemberName] string property = "")
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }

    public long GetViewHashCode() => (Action, key, actionName).GetHashCode();
    
        
    public override string ToString()
    {
        return $"|{ActionName}| - {Key}";
    }


}