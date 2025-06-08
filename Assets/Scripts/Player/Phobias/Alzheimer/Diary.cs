using Cinemachine;
using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class Diary : MonoBehaviour
{
    [SerializeField] private Transform leftPageContainer;
    [SerializeField] private Transform rightPageContainer;
    [SerializeField] private GlobalInteractionGroup globalInteractions;
    [SerializeField] private InteractionHandler interactionHandler;

    private DiaryPage[] leftPages;
    private DiaryPage[] rightPages;
    private PlayerController playerController;
    private CinemachineVirtualCamera virtualCamera;

    private int currentPageIndex = 0;

    private void Awake()
    {
        leftPages = new DiaryPage[leftPageContainer.childCount];
        rightPages = new DiaryPage[rightPageContainer.childCount];

        for (int i = 0; i < leftPageContainer.childCount; i++)
        {
            leftPages[i] = leftPageContainer.GetChild(i).GetComponent<DiaryPage>();
            leftPages[i].gameObject.SetActive(false); // Disable all pages initially
        }

        for (int i = 0; i < rightPageContainer.childCount; i++)
        {
            rightPages[i] = rightPageContainer.GetChild(i).GetComponent<DiaryPage>();
            rightPages[i].gameObject.SetActive(false); // Disable all pages initially
        }

        ShowCurrentPages();
        playerController = FindFirstObjectByType<PlayerController>();
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        var diaryControls = interactionHandler.InputControls.Diary;
        diaryControls.Enable();
        diaryControls.TurnPageLeft.performed += TurnLeft;
        diaryControls.TurnPageRight.performed += TurnRight;
        PlayerCamera.Instance.OnBlendFinished += OnBlendFinished;
        interactionHandler.InputControls.General.Exit.performed += OnExit;
        interactionHandler.SetGlobalInteractions(globalInteractions);
        playerController.enabled = false;
        virtualCamera.Priority = 1;
    }
    
    private void OnDisable()
    {
        var diaryControls = interactionHandler.InputControls.Diary;
        diaryControls.Disable();
        diaryControls.TurnPageLeft.performed -= TurnLeft;
        diaryControls.TurnPageRight.performed -= TurnRight;
        PlayerCamera.Instance.OnBlendFinished -= OnBlendFinished;
        interactionHandler.InputControls.General.Exit.performed -= OnExit;
        playerController.enabled = true;
        virtualCamera.Priority = 0;
    }

    private void OnBlendFinished()
    {
        Debug.Log("OnBlendFinished");
    }

    private void OnExit(CallbackContext _)
    {
        NegateActiveState();
    }

    private void TurnRight(CallbackContext _)
    {
        FlipToNextPage();
    }

    private void TurnLeft(CallbackContext _)
    {
        FlipToPreviousPage();
    }

    public void NegateActiveState()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
    public void FlipToNextPage()
    {
        if (currentPageIndex < leftPages.Length - 1)
        {
            HideCurrentPages();
            currentPageIndex++;
            ShowCurrentPages();
        }
    }

    public void FlipToPreviousPage()
    {
        if (currentPageIndex > 0)
        {
            HideCurrentPages();
            currentPageIndex--;
            ShowCurrentPages();
        }
    }

    public void UnlockPage(int pageIndex)
    {
        if (pageIndex >= 0 && pageIndex < leftPages.Length)
        {
            leftPages[pageIndex].gameObject.SetActive(true);
            rightPages[pageIndex].gameObject.SetActive(true);
        }
    }

    private void ShowCurrentPages()
    {
        leftPages[currentPageIndex].gameObject.SetActive(true);
        rightPages[currentPageIndex].gameObject.SetActive(true);
    }

    private void HideCurrentPages()
    {
        leftPages[currentPageIndex].gameObject.SetActive(true);
        rightPages[currentPageIndex].gameObject.SetActive(true);
    }
}
