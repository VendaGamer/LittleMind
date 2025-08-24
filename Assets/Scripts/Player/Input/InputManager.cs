using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

[DisallowMultipleComponent]
public class InputManager : MonoBehaviour
{
    private static InputManager instance;
    private static PlayerInput PlayerInput;
    public static string CurrentControlScheme { get; private set; }
    public static Controls InputControls { get; private set; }
    public static UnityEvent<string> ControlSchemeChanged { get; } = new();

    private void Awake()
    {
        if(instance)
            Destroy(this);
        
        instance = this;
    }

    private void Start()
    {
        CurrentControlScheme = PlayerInput.currentControlScheme;
        PlayerInput.camera = Camera.main;
        InputControls.General.Enable();
        PlayerInput.onControlsChanged += OnControlsChanged;
        PlayerInput.uiInputModule = FindFirstObjectByType<InputSystemUIInputModule>();
    }

    private void OnDestroy()
    {
        PlayerInput.onControlsChanged -= OnControlsChanged;
        ControlSchemeChanged.RemoveAllListeners();
    }

    private static void OnControlsChanged(PlayerInput playerInput)
    {
        if (CurrentControlScheme == playerInput.currentControlScheme)
            return;
        
        CurrentControlScheme = playerInput.currentControlScheme;
        ControlSchemeChanged.Invoke(CurrentControlScheme);
        Debug.Log($"Changed control scheme {CurrentControlScheme}");
    }
    

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnLoad()
    {
        var gameObject = new GameObject("Input Manager");
        gameObject.AddComponent<InputManager>();
        PlayerInput = gameObject.AddComponent<PlayerInput>();
        InputControls = new Controls();
        PlayerInput.actions = InputControls.asset;
        PlayerInput.neverAutoSwitchControlSchemes = false;
        PlayerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
        PlayerInput.defaultControlScheme = InputControls.controlSchemes[0].name;
    }
}