using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StoryLineManager: MonoBehaviourSingleton<StoryLineManager>
{
    [SerializeField] private Chapter[] chapters;

    private VisualElement gameSavingIndicator;
    
    private byte currentChapter;

    public void NewChapter()
    {
        currentChapter++;
        var chapter = chapters[currentChapter];
        PlayerUIManager.Instance?.NewChapter(currentChapter, chapter.Title);
    }

    public void ShowGameSaving()
    {
        
    }
}