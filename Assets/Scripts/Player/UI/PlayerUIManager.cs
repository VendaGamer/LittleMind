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
        newChapterSeq = DOTween
            .Sequence()
            .Append(chapterPopup.DOFadeIn(1f))
            .Append(chapterPopup.DOFadeOut(3f).SetDelay(5f));
        NewChapter(1, "What is upon us");
    }
}
