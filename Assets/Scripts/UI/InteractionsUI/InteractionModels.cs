using System.Linq;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class UIInteractionItem
{
    public string ActionName { get; set; }
    public string DisplayText { get; set; }
    public bool IsIcon { get; set; }
}

[Serializable]
public class UIInteractionGroup
{
    public string GroupLabel { get; set; }
    public List<UIInteractionItem> Interactions { get; set; } = new List<UIInteractionItem>();
}