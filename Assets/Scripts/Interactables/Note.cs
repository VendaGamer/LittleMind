
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Note : BaseInteractable
{
    [SerializeField] private Transform returnPoint;
    [SerializeField] private NoteData data;
    protected override InteractableData InteractableData => data;

    private Interaction[] interactions;
    public override Interaction[] CurrentInteractions => interactions;
    public override bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (invokedAction.id == data.Use.Action.id)
        {
            Diary.Instance.UnlockNextNote();
            var player = interactor as PlayerController;
            player?.transform.SetPositionAndRotation(returnPoint.position, Quaternion.identity);
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