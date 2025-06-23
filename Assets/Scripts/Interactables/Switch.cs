
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
    private Outline outline;

    public override Interaction[] CurrentInteractions
    {
        get
        {
            return new Interaction[] { };
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
        SwitchOnTween = transform.DOLocalMoveX(data.switchMoveDist, data.switchMoveSpeed);
        outline = GetComponent<Outline>();
    }

    public override void OnStartedToLookAt(IInteractor interactor)
    {
        outline.enabled = true;
    }

    public override void OnEndedToLookAt(IInteractor interactor)
    {
        outline.enabled = false;
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
        if (SwitchOnTween.IsPlaying())
        {
            if (SwitchOnTween.isBackwards)
            {
                SwitchOnTween.Kill();
            }
            else
            {
                return;
            }
        }
        
        SwitchOnTween.Play();
        
    }

    private void SwitchOff()
    {
        isOn = false;
        if (SwitchOnTween.IsPlaying())
        {
            if (!SwitchOnTween.isBackwards)
            {
                SwitchOnTween.Kill();
            }
            else
            {
                return;
            }
        }
        
        SwitchOnTween.PlayBackwards();
    }
}