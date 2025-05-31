using System.Collections.Generic;
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
    
    private VisualElement globalInteractionsContainer;
    private VisualElement currentInteractionsContainer;

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

    public void UpdateGlobalInteractions(List<VisualElement> interactions)
    {
        // Clear existing global interactions
        globalInteractionsContainer.Clear();
        
        // Add new global interactions
        if (interactions != null)
        {
            foreach (var interaction in interactions)
            {
                globalInteractionsContainer.Add(interaction);
            }
        }
    }

    public void UpdateCurrentInteractions(List<VisualElement> interactions)
    {
        // Clear existing current interactions
        currentInteractionsContainer.Clear();
        
        // Add new current interactions
        if (interactions != null)
        {
            foreach (var interaction in interactions)
            {
                currentInteractionsContainer.Add(interaction);
            }
        }
        
        // Show/hide the container based on whether there are interactions
        bool hasInteractions = interactions != null && interactions.Count > 0;
        currentInteractionsContainer.style.display = hasInteractions ? DisplayStyle.Flex : DisplayStyle.None;
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
        globalInteractionsContainer = root.Q<VisualElement>("global-interactions-container");
        currentInteractionsContainer = root.Q<VisualElement>("current-interactions-container");
        
        newChapterSeq = DOTween
            .Sequence()
            .Append(chapterPopup.DOFadeIn(1f))
            .Append(chapterPopup.DOFadeOut(3f).SetDelay(5f));
        NewChapter(1, "What is upon us");
    }
}
