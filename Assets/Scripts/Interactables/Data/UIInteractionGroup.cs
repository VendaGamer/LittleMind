using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a group of UI interactions that can be used in the game.
/// </summary>
[Serializable]
public class UIInteractionGroup
{
    [SerializeField] private string groupName;
    [SerializeField] private List<UIInteraction> interactions = new List<UIInteraction>();
    
    public string GroupName => groupName;
    public List<UIInteraction> Interactions => interactions;
}

/// <summary>
/// Represents an individual UI interaction that can be performed.
/// </summary>
[Serializable]
public class UIInteraction
{
    [SerializeField] private string interactionName;
    [SerializeField] private KeyCode keyCode;
    [SerializeField] private string description;
    
    public string InteractionName => interactionName;
    public KeyCode KeyCode => keyCode;
    public string Description => description;
}
