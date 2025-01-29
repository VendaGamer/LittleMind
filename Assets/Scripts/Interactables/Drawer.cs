using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Drawer : MonoBehaviour, IInteractable
{
    [field:SerializeField] public string InteractGroupLabel { get; private set; }
    [SerializeField] private float lerpDur = 1f;
    [SerializeField] private Interaction openInteraction;
    [SerializeField] private Interaction closeInteraction;
    [SerializeField] private float openX = -1.5f;
    private bool isOpen = false;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    
    private void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition + new Vector3(openX, 0f, 0f);
    }
    public Interaction[] CurrentInteractions
    {
        get
        {
            return isOpen ? new[] { closeInteraction } : new[] { openInteraction};
        }
    }

    private void Open()
    {
        StopAllCoroutines();
        StartCoroutine(OpenRoutine());
    }

    private void Close()
    {
        StopAllCoroutines();
        StartCoroutine(CloseRoutine());
    }
    private IEnumerator OpenRoutine()
    {
        Vector3 startPos = transform.position;
        float distanceToMove = Vector3.Distance(startPos, openPosition);
        float adjustedDuration = lerpDur * (distanceToMove / Mathf.Abs(openX));
    
        float elapsedTime = 0f;
        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.SmoothStep(0, 1, elapsedTime / adjustedDuration);

            transform.position = Vector3.Lerp(startPos, openPosition, step);
            yield return null;
        }

        transform.position = openPosition;
    }

    private IEnumerator CloseRoutine()
    {
        Vector3 startPos = transform.position;
        float distanceToMove = Vector3.Distance(startPos, closedPosition);
        float adjustedDuration = lerpDur * (distanceToMove / Mathf.Abs(openX));
    
        float elapsedTime = 0f;
        while (elapsedTime < adjustedDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.SmoothStep(0, 1, elapsedTime / adjustedDuration);

            transform.position = Vector3.Lerp(startPos, closedPosition, step);
            yield return null;
        }

        transform.position = closedPosition;
    }
    
    public bool Interact(IInteractor interactor, InputAction invokedAction)
    {
        if (isOpen)
        {
            if (invokedAction.id == closeInteraction.Action.action.id)
            {
                Close();
                isOpen = false;
                return true;
            }
        }
        else
        {
            Open();
            isOpen = true;
            return true;
        }
        return false;
    }

}