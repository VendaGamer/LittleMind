using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private PlayerController playerController;
    
    private UIInteractionsData uiData;
    private VisualElement menuContainer;
    
    private void OnEnable()
    {
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
        
        uiData = playerController.GetComponent<UIInteractionsData>();
        if (uiData == null)
        {
            Debug.LogError("UIInteractionsData component not found on PlayerController!");
            return;
        }
        
        uiData.OnInteractionsChanged += RefreshUI;
        
        InitializeUI();
    }
    
    private void OnDisable()
    {
        if (uiData != null)
        {
            uiData.OnInteractionsChanged -= RefreshUI;
        }
    }
    
    private void InitializeUI()
    {
        if (uiDocument == null) return;
        
        var root = uiDocument.rootVisualElement;
        menuContainer = root.Q<VisualElement>("menu-container");
        
        // Set the data source
        root.userData = uiData;
        root.dataSource = uiData;
        
        RefreshUI();
    }
    
    private void RefreshUI()
    {
        if (menuContainer == null) return;
        
        // Show/hide the menu based on whether there are any interactions to display
        menuContainer.style.display = uiData.Groups.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;
    }
}