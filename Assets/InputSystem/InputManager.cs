using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private PlayerInput playerInput;
    public static InputManager Instance { get; private set; }
    public ControlScheme CurrentControlScheme { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        playerInput.onControlsChanged += OnControlsChanged;
    }

    private void OnDisable()
    {
        playerInput.onControlsChanged -= OnControlsChanged;
    }
    private void OnControlsChanged(PlayerInput obj)
    {
        if (obj.currentControlScheme == "KeyboardMouse")
        {
            CurrentControlScheme = ControlScheme.KeyboardMouse;
        }
        else if (obj.currentControlScheme == "Gamepad")
        {
            CurrentControlScheme = ControlScheme.Gamepad;
        }
    }

    public enum ControlScheme
    {
        KeyboardMouse,
        Gamepad
    }
}
