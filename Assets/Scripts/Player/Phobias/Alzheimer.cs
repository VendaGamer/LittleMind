using JetBrains.Annotations;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;
public class Alzheimer : MonoBehaviour
{
    [SerializeField]
    private Diary playerDiary;
    [CanBeNull]
    private MemoryTrigger currentMemoryTrigger;
    
    [Header("Interaction Settings")]
    [SerializeField]
    private InteractionHandler interactionHandler;

    public void RegisterMemoryTrigger(MemoryTrigger trigger) =>
        currentMemoryTrigger = trigger;

    public void UnregisterMemoryTrigger(MemoryTrigger trigger)
    {
        if (ReferenceEquals(trigger, currentMemoryTrigger))
        {
            currentMemoryTrigger = null;
        }
    }

    private void Update()
    {
        if (!currentMemoryTrigger)
        {
            return;
        }

        PlayerUIManager.Instance.MemoryIconVisibility =
            GeometryUtility.TestPlanesAABB(PlayerCamera.Instance.FrustumPlanes, currentMemoryTrigger.BoundsToLookAt);
    }

    private void OnEnable()
    {
        interactionHandler.InputControls.Player.Journal.performed += OnJournal;
    }

    private void OnDisable()
    {
        interactionHandler.InputControls.Player.Journal.performed -= OnJournal;
    }

    private void OnJournal(CallbackContext _)
    {
        playerDiary.NegateActiveState();
    }
}
