using System;
using Unity.Cinemachine;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

[DefaultExecutionOrder(300)]
public class Diary : MonoBehaviour
{
    [SerializeField]
    private Transform leftPageContainer;

    [SerializeField]
    private Transform rightPageContainer;

    [SerializeField]
    private GlobalInteractionGroup globalInteractions;
    
    private PlayerController playerController;
    private CinemachineCamera virtualCamera;
    
    [SerializeField]
    private Outline[] notes;

    private int unlockedNoteCount = 3;
    
    private int currentNoteIndex;

    private void Awake()
    {
        HighlightCurrentNote();
        playerController = FindFirstObjectByType<PlayerController>();
        virtualCamera = GetComponentInChildren<CinemachineCamera>();
    }

    private void OnEnable()
    {
        var diaryControls = InputManager.InputControls.Diary;
        PlayerUIManager.Instance.CrosshairVisibility = false;
        diaryControls.Enable();
        diaryControls.Navigate.performed += OnNavigate;
        diaryControls.Exit.performed += OnExit;
        
        PlayerCamera.Instance.OnBlendFinished += OnBlendFinished;
        InputManager.InputControls.General.Exit.performed += OnExit;
        
        playerController.enabled = false;
        virtualCamera.Priority = PlayerCamera.Instance.CurrentVirtualCameraPriority + 2;
    }

    private void OnDisable()
    {
        PlayerUIManager.Instance.CrosshairVisibility = true;
        var diaryControls = InputManager.InputControls.Diary;
        diaryControls.Disable();
        diaryControls.Navigate.performed -= OnNavigate;
        diaryControls.Exit.performed -= OnExit;
        
        PlayerCamera.Instance.OnBlendFinished -= OnBlendFinished;

        playerController.enabled = true;
        virtualCamera.Priority = PlayerCamera.Instance.CurrentVirtualCameraPriority - 2;
    }

    private void OnNavigate(CallbackContext obj)
    {
        var dir = obj.ReadValue<Vector2>();

        if (dir.x > 0.5f) // right
            currentNoteIndex++;
        else if (dir.x < -0.5f) // left
            currentNoteIndex--;

        currentNoteIndex = Mathf.Clamp(currentNoteIndex, 0, unlockedNoteCount - 1);

        HighlightCurrentNote();
    }

    private void HighlightCurrentNote()
    {
        for (int i = 0; i < notes.Length; i++)
            notes[i].enabled = (i == currentNoteIndex);
    }

    private void OnBlendFinished()
    {
        Debug.Log("OnBlendFinished");
    }

    private void OnExit(CallbackContext _)
    {
        NegateActiveState();
    }

    public void NegateActiveState()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void UnlockNextNote()
    {
        notes[unlockedNoteCount - 1].transform.parent.gameObject.SetActive(true);
        unlockedNoteCount++;
    }
}
