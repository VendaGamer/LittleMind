using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class HintManager : MonoBehaviour
{
    public static HintManager Instance { get; private set; }

    [SerializeField] private UIDocument contextMenuDocument;
    private VisualElement menuContainer;
    private Dictionary<Interaction, VisualElement> activeHints = new Dictionary<Interaction, VisualElement>();
    private Interaction[] currentGlobalInteractions;

    private void Awake()
    {
        Instance = this;

        InitializeUI();
    }

    private void OnEnable()
    {
        PlayerController.GlobalInteractionsChanged += OnGlobalHintsChanged;
        PlayerController.ExclusiveInteractionsChanged += OnExclusiveHintsChanged;
    }

    private void OnDisable()
    {
        PlayerController.GlobalInteractionsChanged -= OnGlobalHintsChanged;
        PlayerController.ExclusiveInteractionsChanged -= OnExclusiveHintsChanged;
    }

    private void InitializeUI()
    {
        var root = contextMenuDocument.rootVisualElement;
        menuContainer = root.Q<VisualElement>("menu-container");
        menuContainer.style.display = DisplayStyle.None;
    }

    private void OnGlobalHintsChanged(Interaction[] newGlobalInteractions)
    {
        
    }
    
    private void OnExclusiveHintsChanged(Interaction[] newExclusiveInteractions)
    {
        
    }
    
    public void UpdateInteractionHints(Interaction[] availableInteractions)
    {
        if (availableInteractions.Length == 0)
        {
            menuContainer.style.display = DisplayStyle.None;
            ClearHints();
            return;
        }

        menuContainer.style.display = DisplayStyle.Flex;
    }

    private VisualElement CreateHintElement(Interaction interaction)
    {
        var container = new VisualElement();
        container.AddToClassList("menu-item");

        var actionText = new Label(interaction.);
        actionText.AddToClassList("action-text");

        var inputDisplay = interaction.;
        var keyHint = new VisualElement();
        keyHint.AddToClassList("key-hint");

        // Handle different input display types
        if (!string.IsNullOrEmpty(inputDisplay.IconPath))
        {
            var icon = new Image();
            icon.sprite = Resources.Load<Sprite>(inputDisplay.IconPath);
            keyHint.Add(icon);
        }
        else
        {
            var textHint = new Label(inputDisplay.Text);
            keyHint.Add(textHint);
        }

        container.Add(actionText);
        container.Add(keyHint);
        return container;
    }

    private void ClearHints()
    {
        menuContainer.Clear();
        activeHints.Clear();
    }

    public void RefreshAllHints()
    {
        throw new System.NotImplementedException();
    }
}