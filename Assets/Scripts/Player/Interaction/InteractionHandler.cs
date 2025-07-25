using System;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[GeneratePropertyBag]
public partial class InteractionHandler : ScriptableObject, INotifyBindablePropertyChanged, IDataSourceViewHashProvider
{
    [CreateProperty]
    public string CurrentInteractionGroupLabel
    {
        get => currentInteractionGroupLabel;
        private set
        {
            currentInteractionGroupLabel = value;
            Notify();
        }
    }
    
    [CreateProperty]
    public Interaction[] CurrentInteractions
    {

        get => currentInteractions;
        private set
        {
            currentInteractions = value;
            Notify();
        }
    }
    
    public static InteractionHandler Instance { get; private set; }
    
    [CanBeNull]
    private IInteractable shownInteractable;
    
    private string currentInteractionGroupLabel;
    private Interaction[] currentInteractions;
    public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

    public void SetCurrentInteractable([CanBeNull] IInteractable newInteractable)
    {
        if (ReferenceEquals(shownInteractable, newInteractable))
            return;

        //unregister current
        if (shownInteractable != null)
            shownInteractable.InteractionsChanged -= OnInteractionsChanged;


        PlayerUIManager.Instance.HideInteractableContainer();
        if (newInteractable == null)
        {
            CurrentInteractionGroupLabel = null;
            CurrentInteractions = null;
            shownInteractable = null;
            return;
        }
        
        CurrentInteractionGroupLabel = newInteractable.InteractGroupLabel;
        CurrentInteractions = newInteractable.CurrentInteractions;
        newInteractable.RebuildKeys();
        PlayerUIManager.Instance.ShowInteractableContainer();
        shownInteractable = newInteractable;
        shownInteractable.InteractionsChanged += OnInteractionsChanged;
    }

    private void OnInteractionsChanged()
    { 

        CurrentInteractions = shownInteractable?.CurrentInteractions;
    }


    private void OnEnable()
    {
        Instance = this;
        InputManager.ControlSchemeChanged.AddListener(RefreshUI);
    }
    
    private void OnDisable()
    {
        InputManager.ControlSchemeChanged.RemoveListener(RefreshUI);
    }

    private void RefreshUI(string _)
    {
        if (CurrentInteractions == null)
            return;
        
        foreach (var interaction in CurrentInteractions)
        {
            interaction.RebuildKey();
        }
    }
    
    public long GetViewHashCode() => (currentInteractionGroupLabel, currentInteractions).GetHashCode();

    private void Notify([CallerMemberName] string property = "")
    {
        propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
    }
}