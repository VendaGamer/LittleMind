using DG.Tweening;
using Humanizer;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviourSingleton<PlayerUIManager>
{
    [SerializeField]
    private UIDocument playerUI;
    private VisualElement chapterPopup;
    private Label chapterTitle;
    private Label chapterLabel;
    private VisualElement memoryIcon;
    private VisualElement heartIcon;
    private Sequence newChapterSeq;
    private ListView interactableListView;
    private VisualElement interactableInteractions;
    private VisualElement statusBar;
    private VisualElement crossHair;

    public bool MemoryIconVisibility
    {
        get => memoryIcon.visible;
        set
        {
            if (value == memoryIcon.visible)
                return;
            
            memoryIcon.visible = value;
            
            if (value == heartIcon.visible)
            {
                statusBar.visible = value;
            }
        }
    }
    
    public bool Visibility
    {
        get => playerUI.rootVisualElement.visible;
        set => playerUI.rootVisualElement.visible = value;
    }

    public void NewChapter(int chapterNum, string contents)
    {
        chapterTitle.text = chapterNum.ToRoman();
        chapterLabel.text = contents;
        newChapterSeq.Play();
    }

    public void RefreshInteractionsListView()
    {
        interactableListView.Rebuild();
    }
    public void ShowInteractableContainer()
    {
        interactableInteractions.visible = true;
        crossHair.AddToClassList("crosshair--interactive");
    }
    
    public void HideInteractableContainer()
    {
        interactableInteractions.visible = false;
        crossHair.RemoveFromClassList("crosshair--interactive");
    }

    protected override void Awake()
    {
        base.Awake();
        var root = playerUI.rootVisualElement;
        chapterPopup = root.Q<VisualElement>("chapter-popup");
        chapterTitle = root.Q<Label>("chapter-title");
        chapterLabel = root.Q<Label>("chapter-label");
        memoryIcon = root.Q<VisualElement>("bulb-icon");
        heartIcon = root.Q<VisualElement>("heart-icon");
        crossHair = root.Q<VisualElement>("crosshair");
        interactableListView = root.Q<ListView>("interactable-listview");
        interactableInteractions = root.Q<VisualElement>("interactable-interactions");
        statusBar = root.Q<VisualElement>("status-bar");
        interactableInteractions.visible = false;
        memoryIcon.visible = false;
        heartIcon.visible = false;
        statusBar.visible = false;
        
        newChapterSeq = DOTween
            .Sequence()
            .Append(chapterPopup.DOFadeIn(1f))
            .Append(chapterPopup.DOFadeOut(3f).SetDelay(5f));
        NewChapter(1, "What is upon us");
    }
}