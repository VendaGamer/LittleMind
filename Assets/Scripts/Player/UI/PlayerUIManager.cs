using DG.Tweening;
using Humanizer;
using UnityEngine;
using UnityEngine.UIElements;
using ZLinq;

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
    
    private VisualElement[] statusBarElements;

    private Tweener statusBarTweener;

    public bool MemoryIconVisibility
    {
        get => memoryIcon.visible;
        set
        {
            if (value == memoryIcon.visible)
                return;
            
            
        }
    }

    public bool HeartIconVisibility
    {
        get => heartIcon.visible;
        set
        {
            if (value == heartIcon.visible)
                return;
            
            
        }
    }
    
    public bool Visibility
    {
        get => playerUI.rootVisualElement.visible;
        set => playerUI.rootVisualElement.visible = value;
    }
    

    private bool IsAnyElementVisible()
    {
        return statusBarElements.AsValueEnumerable().Any(e => e.visible);
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

    private void FadeStatBarElement(VisualElement element)
    {
        if (element.visible)
        {
            
        }
        else
        {
            
        }
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
        
        // Initialize status bar and elements as invisible
        statusBar.visible = false;
        statusBar.style.opacity = 0f;
        memoryIcon.visible = false;
        memoryIcon.style.opacity = 0f;
        heartIcon.visible = false;
        heartIcon.style.opacity = 0f;

        statusBarElements = new[] { heartIcon, memoryIcon };
        
        newChapterSeq = DOTween
            .Sequence()
            .Append(chapterPopup.DOFadeIn(1f))
            .Append(chapterPopup.DOFadeOut(3f).SetDelay(5f));
        
        NewChapter(1, "What is upon us");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Clean up tweeners
        statusBarTweener?.Kill();
        newChapterSeq?.Kill();
    }
}