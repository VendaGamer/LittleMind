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
    public VisualElement CurrentInteractionsContainer { get; private set; }
    public Label CurrentInteractionsLabel { get; private set; }
    
    private VisualElement interactionGroupsContainer;
    private VisualElement currentInteractionsGroup;
    private VisualElement globalInteractionsGroup;

    public bool MemoryIconVisibility
    {
        get => memoryIcon.visible;
        set => memoryIcon.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
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
        
        globalInteractionsGroup.style.display = DisplayStyle.Flex;
        interactionGroupsContainer.style.display = DisplayStyle.Flex;
    }

    public void SetCurrentInteractions(VisualElement[] interactions)
    {
        CurrentInteractionsContainer.Clear();

        foreach (var interaction in interactions)
        {
            CurrentInteractionsContainer.Add(interaction);
        }
        
        currentInteractionsGroup.style.display = DisplayStyle.Flex;
        interactionGroupsContainer.style.display = DisplayStyle.Flex;
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
        currentInteractionsGroup = interactionGroupsContainer.Q<VisualElement>("current-interactions-group");

        
        GlobalInteractionsLabel = globalInteractionsGroup.Q<Label>("global-interactions-label");
        CurrentInteractionsLabel = currentInteractionsGroup.Q<Label>("current-interactions-label");
        
        GlobalInteractionsContainer = globalInteractionsGroup.Q<VisualElement>("global-interactions-container");
        CurrentInteractionsContainer = currentInteractionsGroup.Q<VisualElement>("current-interactions-container");
        
        newChapterSeq = DOTween
            .Sequence()
            .Append(chapterPopup.DOFadeIn(1f))
            .Append(chapterPopup.DOFadeOut(3f).SetDelay(5f));
        NewChapter(1, "What is upon us");
    }

    public void ClearGlobalInteractions()
    {
        GlobalInteractionsContainer.Clear();
        GlobalInteractionsContainer.style.display = DisplayStyle.None;
        if (CurrentInteractionsContainer.style.display == DisplayStyle.None)
            interactionGroupsContainer.style.display = DisplayStyle.Flex;
    }

    public void HideUI()
    {
        
    }

    public void ClearCurrentInteractableInteractions()
    {
        CurrentInteractionsContainer.Clear();
        CurrentInteractionsContainer.style.display = DisplayStyle.None;
        if (GlobalInteractionsContainer.style.display == DisplayStyle.None)
            interactionGroupsContainer.style.display = DisplayStyle.Flex;
    }
}
