using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StoryLineManager: MonoBehaviour, ISingleton
{
    [CanBeNull]
    public static StoryLineManager Instance { get; private set; }

    [SerializeField] private Chapter[] chapters;
    [SerializeField] private UIDocument playerUI;
    private Label newChapterName;
    private VisualElement gameSavingIndicator;
    
    private byte currentChapter;

    public void NewChapter()
    {

        newChapterName.DOFade(0f, 2f);
        currentChapter++;
    }

    public void ShowGameSaving()
    {
        
    }

    public void Awake()
    {
        newChapterName = playerUI.rootVisualElement.Q<Label>("NewChapterName");
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
}