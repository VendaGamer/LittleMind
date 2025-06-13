using UnityEngine;

public class PlayerInput : UnityEngine.InputSystem.PlayerInput
{
    [Header("Interaction")]
    [SerializeField]
    private InteractionHandler interactionHandler;
    private void OnEnable()
    {
        onControlsChanged += interactionHandler.OnControlsChanged;
    }
    private void OnDisable()
    {
        onControlsChanged -= interactionHandler.OnControlsChanged;
    }
}
