using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drawer : MonoBehaviour, IInteractable, IDisposable
{
    [SerializeField] private DrawerInfo info;
    [field:SerializeField] public string InteractGroupLabel { get; private set; }
    private bool isOpen = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private CancellationTokenSource curTokSrc;
    
    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(info.OpenX, 0f, 0f) * transform.parent.localScale.x;
    }
    public Interaction[] CurrentInteractions
    {
        get
        {
            return isOpen ? new[] { info.CloseDrawerInteraction } : new[] {info.OpenDrawerInteraction};
        }
    }
    
    private async Task Move(Vector3 destination,CancellationToken ctk)
    {
        Vector3 startPos = transform.position;
        float distanceToMove = Vector3.Distance(startPos, destination);
        float adjustedDuration = info.LerpDuration * (distanceToMove / Mathf.Abs(info.OpenX));
    
        float elapsedTime = 0f;
        while (elapsedTime < adjustedDuration)
        {
            ctk.ThrowIfCancellationRequested();
            elapsedTime += Time.deltaTime;
            float step = Mathf.SmoothStep(0, 1, elapsedTime / adjustedDuration);

            transform.position = Vector3.Lerp(startPos, destination, step);
            await Task.Yield();
        }

        transform.position = destination;
    }
    private void CancelCurrentAction()
    {
        if (curTokSrc == null)
            return;
        
        curTokSrc.Cancel();
        curTokSrc.Dispose();
        curTokSrc = null;
    }
    
    public bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (isOpen)
        {
            if (invokedAction.id == info.CloseDrawerInteraction.Action.action.id)
            {
                CancelCurrentAction();
                curTokSrc = new CancellationTokenSource();
                Move(closedPosition, curTokSrc.Token);
                isOpen = false;
                return true;
            }
        }
        else if(invokedAction.id == info.OpenDrawerInteraction.Action.action.id)
        {
            CancelCurrentAction();
            curTokSrc = new CancellationTokenSource();
            Move(openPosition, curTokSrc.Token);
            isOpen = true;
            return true;
        }
        return false;
    }

    public void Dispose()
    {
        curTokSrc?.Dispose();
    }
}