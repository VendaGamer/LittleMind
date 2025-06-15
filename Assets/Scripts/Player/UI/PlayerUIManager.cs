using DG.Tweening;
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
    
    public VisualElement GlobalInteractionsContainer { get; private set; }
    public Label GlobalInteractionsLabel { get; private set; }
    public VisualElement InteractableInteractionsContainer { get; private set; }
    public Label CurrentInteractionsLabel { get; private set; }
    
    private VisualElement interactionGroupsContainer;
    private VisualElement interactableInteractionsGroup;
    private VisualElement globalInteractionsGroup;

    public bool MemoryIconVisibility
    {
        get => memoryIcon.visible; 
        set => memoryIcon.visible = value;
    }
    
    public bool Visibility
    {
        get => playerUI.rootVisualElement.visible;
        set => playerUI.rootVisualElement.visible = value;
    }

    public void NewChapter(byte chapterNum, string contents)
    {
        chapterLabel.text = $"Chapter {chapterNum}";
        chapterTitle.text = contents;
        newChapterSeq.Play();
        
    }

    public void SetGlobalInteractions(VisualElement[] interactions)
    {
        GlobalInteractionsContainer.Clear();
        
        foreach (var interaction in interactions)
        {
            GlobalInteractionsContainer.Add(interaction);
        }
        
        globalInteractionsGroup.visible = true;
        interactionGroupsContainer.visible = true;
    }

    public void SetCurrentInteractions(VisualElement[] interactions)
    {
        InteractableInteractionsContainer.Clear();

        foreach (var interaction in interactions)
        {
            InteractableInteractionsContainer.Add(interaction);
        }
        
        InteractableInteractionsContainer.visible = true;
        interactionGroupsContainer.visible = true;
    }


    protected override void Awake()
    {
        base.Awake();
        
        var root = playerUI.rootVisualElement;
        chapterPopup = root.Q<VisualElement>("chapter-popup");
        chapterTitle = root.Q<Label>("chapter-title");
        chapterLabel = root.Q<Label>("chapter-label");
        memoryIcon = root.Q<VisualElement>("memory-icon");
        heartIcon = root.Q<VisualElement>("health-icon");
        memoryIcon.style.display = DisplayStyle.None;
        heartIcon.style.display = DisplayStyle.None;
        
        interactionGroupsContainer = root.Q<VisualElement>("interaction-groups-container");
        
        globalInteractionsGroup = interactionGroupsContainer.Q<VisualElement>("global-interactions-group");
        interactableInteractionsGroup = interactionGroupsContainer.Q<VisualElement>("current-interactions-group");

        
        GlobalInteractionsLabel = globalInteractionsGroup.Q<Label>("global-interactions-label");
        CurrentInteractionsLabel = interactableInteractionsGroup.Q<Label>("current-interactions-label");
        
        GlobalInteractionsContainer = globalInteractionsGroup.Q<VisualElement>("global-interactions-container");
        InteractableInteractionsContainer = interactableInteractionsGroup.Q<VisualElement>("current-interactions-container");
        
        newChapterSeq = DOTween
            .Sequence()
            .Append(chapterPopup.DOFadeIn(1f))
            .Append(chapterPopup.DOFadeOut(3f).SetDelay(5f));
        NewChapter(1, "What is upon us");
    }

    public void ClearGlobalInteractions()
    {
        GlobalInteractionsContainer.Clear();
        GlobalInteractionsContainer.visible = true;
        if (!InteractableInteractionsContainer.visible)
            interactionGroupsContainer.visible = false;
    }

    public void ClearCurrentInteractableInteractions()
    {
        InteractableInteractionsContainer.Clear();
        InteractableInteractionsContainer.visible = true;
        if (!GlobalInteractionsContainer.visible)
            interactionGroupsContainer.visible = false;
    }
}
