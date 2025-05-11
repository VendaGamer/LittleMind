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

    public bool MemoryIconVisibility
    {
        get => memoryIcon.visible;
        set => memoryIcon.visible = value;
    }
    
    
    public void NewChapter(byte chapterNum,string contents)
    {
        chapterLabel.text = $"Chapter {chapterNum}";
        chapterTitle.text = contents;
    }

    protected override void Awake()
    {
        base.Awake();
        
        var root = playerUI.rootVisualElement;
        chapterPopup = root.Q<VisualElement>("chapter-popup");
        chapterTitle = root.Q<Label>("chapter-title");
        chapterLabel = root.Q<Label>("chapter-label");
        memoryIcon = root.Q<VisualElement>("memory-icon");
    }
}