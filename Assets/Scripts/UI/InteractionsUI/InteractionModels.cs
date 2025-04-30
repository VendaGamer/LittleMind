using System;
using System.Collections.Generic;
using Unity.Properties;

[Serializable]
public class UIInteractionItem
{
    public string ActionName;
    public string DisplayText;
    public bool IsIcon;

    [CreateProperty]
    public bool IsNotIcon => !IsIcon;
}

[Serializable]
public class UIInteractionGroup
{
    public string GroupLabel;
    public List<UIInteractionItem> Interactions = new();
}
