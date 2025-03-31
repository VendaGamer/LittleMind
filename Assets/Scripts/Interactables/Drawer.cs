using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drawer : MonoBehaviour, IInteractable, IDisposable
{
    [SerializeField] private DrawerData data;
    [field:SerializeField] public string InteractGroupLabel { get; private set; }
    private bool isOpen = false;
    private Vector3 closedPosition;
    private Coroutine currentMoveCoroutine;
    private Outline outline;
    
    private void Start()
    {
        closedPosition = transform.position;
        outline = GetComponent<Outline>();
    }
    
    public Interaction[] CurrentInteractions
    {
        get
        {
            return isOpen ? new[] { data.CloseDrawerInteraction } : new[] {data.OpenDrawerInteraction};
        }
    }
    
    private IEnumerator Move(Vector3 destination)
    {
        Vector3 startPos = transform.position;
        float distanceToMove = Vector3.Distance(startPos, destination);
        float adjustedDuration = data.LerpDuration * (distanceToMove / Mathf.Abs(data.OpenX));
    
        float elapsedTime = 0f;
        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.SmoothStep(0, 1, elapsedTime / adjustedDuration);

            transform.position = Vector3.Lerp(startPos, destination, step);
            yield return null;
        }

        transform.position = destination;
    }
    
    public bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (isOpen)
        {
            if (invokedAction.id == data.CloseDrawerInteraction.ActionRef.action.id)
            {
                if (currentMoveCoroutine != null)
                    StopCoroutine(currentMoveCoroutine);
                    
                currentMoveCoroutine = StartCoroutine(Move(closedPosition));
                isOpen = false;
                return true;
            }
        }
        else if(invokedAction.id == data.OpenDrawerInteraction.ActionRef.action.id)
        {
            if (currentMoveCoroutine != null)
                StopCoroutine(currentMoveCoroutine);
                
            currentMoveCoroutine = StartCoroutine(Move(
                closedPosition + new Vector3(data.OpenX, 0f, 0f) * transform.parent.localScale.x));
            isOpen = true;
            return true;
        }
        return false;
    }

    public bool ToggleOutline(bool value)
    {
        outline.enabled = value;
        return true;
    }

    public void Dispose()
    {
        if (currentMoveCoroutine != null)
            StopCoroutine(currentMoveCoroutine);
    }
}