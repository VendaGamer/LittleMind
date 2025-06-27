
using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Switch : BaseInteractable
{
    [SerializeField]
    private SwitchData data;
    private Tween SwitchOnTween;

    protected override InteractableData InteractableData => data;

    public override Interaction[] CurrentInteractions
    {
        get
        {
            return new Interaction[] { isOn ? data.switchOff : data.switchOn };
        }
    }
    
    private bool isOn = false;

    protected bool IsOn
    {
        get => isOn;
        set
        {
            if(value == isOn) return;
            isOn = value;
            OnInteractionsChanged();
        }
    }

    private void Awake()
    {
        SwitchOnTween = transform.DOLocalMoveX(data.switchMoveDist, data.switchMoveSpeed)
            .SetAutoKill(false)
            .Pause();
    }


    public override bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (isOn)
        {
            if (data.switchOff.Action.id == invokedAction.id)
            {
                SwitchOff();
                return true;
            }
        }
        
        if (data.switchOn.Action.id == invokedAction.id)
        {
            SwitchOn();
            return true;
        }
        
        return false;
    }

    private void SwitchOn()
    {
        isOn = true;
        SwitchOnTween.Play();
        
    }

    private void SwitchOff()
    {
        isOn = false;
        
        SwitchOnTween.PlayBackwards();
    }
}