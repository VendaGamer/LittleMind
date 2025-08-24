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
    
    private int currentNoteIndex;

    private void Awake()
    {
        playerController = FindFirstObjectByType<PlayerController>();
        virtualCamera = GetComponentInChildren<CinemachineCamera>();
    }

    private void OnEnable()
    {
        InputManager.InputControls.General.Enable();
        var diaryControls = InputManager.InputControls.Diary;
        diaryControls.Enable();
        diaryControls.Navigate.performed += OnNavigate;
        PlayerCamera.Instance.OnBlendFinished += OnBlendFinished;
        InputManager.InputControls.General.Exit.performed += OnExit;
        playerController.enabled = false;
        virtualCamera.Priority = PlayerCamera.Instance.CurrentVirtualCameraPriority + 2;
    }

    private void OnDisable()
    {
        InputManager.InputControls.General.Disable();
        var diaryControls = InputManager.InputControls.Diary;
        diaryControls.Disable();
        diaryControls.Navigate.performed -= OnNavigate;
        PlayerCamera.Instance.OnBlendFinished -= OnBlendFinished;
        InputManager.InputControls.General.Exit.performed -= OnExit;
        playerController.enabled = true;
        virtualCamera.Priority = PlayerCamera.Instance.CurrentVirtualCameraPriority - 2;
    }

    private void OnNavigate(CallbackContext obj)
    {
        
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
    
    

    public void UnlockNote(int noteIndex)
    {
        
    }
}
