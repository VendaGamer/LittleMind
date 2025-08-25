
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Goal : BaseInteractable
{
    [SerializeField] private GameObject rootOfObjectToDisable;
    
    [SerializeField]
    private NoteData data;
    protected override InteractableData InteractableData => data;

    private Interaction[] interactions;
    public override Interaction[] CurrentInteractions => interactions;
    public override bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (invokedAction.id == data.Use.Action.id)
        {
            rootOfObjectToDisable.SetActive(false);
            Destroy(gameObject);
            return true;
        }

        return false;
    }

    private void Awake()
    {
        interactions = new[] { data.Use };
    }
}