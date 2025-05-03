using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class StoryLineManager: MonoSingleton<StoryLineManager>
{
    [SerializeField] private Chapter[] chapters;

    private VisualElement gameSavingIndicator;
    
    private byte currentChapter;

    public void NewChapter()
    {
        currentChapter++;
        PlayerUIManager.Instance?.NewChapter(chapters[currentChapter].Title);
    }

    public void ShowGameSaving()
    {
        
    }
}