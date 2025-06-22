using JetBrains.Annotations;
using Unity.Properties;
using UnityEngine;

[CreateAssetMenu(menuName = "Interactions/Interactions",fileName = "Interactions")]
public class InteractionHandler : ScriptableObject
{
    [CreateProperty]
    public bool CanInteract => currentInteractable is not null;
    public static InteractionHandler Instance { get; private set; }
    public Controls InputControls { get; private set; }
    
    
    private IInteractable currentInteractable;
    private GlobalInteractionGroup currentGlobalInteractionGroup;

    private string _currentControlSchemeName;
    public string currentControlSchemeName
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
        Instance = this;
        InputControls = new Controls();
        currentControlSchemeName = InputControls.KeyboardMouseScheme.name;
    }
    
    private void RefreshUI()
    {
        SetGlobalInteractions(currentGlobalInteractionGroup);
        SetCurrentInteractableInteractions(currentInteractable);
    }
    
    public void SetGlobalInteractions([CanBeNull] GlobalInteractionGroup newGlobalInteractions)
    {
        currentGlobalInteractionGroup = newGlobalInteractions;
    }

    public void SetCurrentInteractableInteractions([CanBeNull] IInteractable interactions)
    {
       currentInteractable = interactions;
    }
    
    public void OnControlsChanged(UnityEngine.InputSystem.PlayerInput playerInput)
        => currentControlSchemeName = playerInput.currentControlScheme;



}