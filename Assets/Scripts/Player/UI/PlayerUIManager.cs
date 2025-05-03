using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class PlayerUIManager : MonoBehaviour
{
    [CanBeNull]
    public static PlayerUIManager Instance { get; private set; }
    
    [SerializeField]
    private UIDocument playerUI;
    private Label newChapterName;
    
    
    public void NewChapter(string contents)
    {
        
    }

    private void Awake()
    {
        newChapterName = playerUI.rootVisualElement.Q<Label>("NewChapterName");
        if (Instance)
        {
            Destroy(Instance);
        }
        Instance = this;
    }
}