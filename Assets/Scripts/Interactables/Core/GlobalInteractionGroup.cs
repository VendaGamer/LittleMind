using System;
using UnityEngine;

[Serializable]
public class GlobalInteractionGroup : IInteractionGroup
{
    [field: SerializeField]
    public string InteractGroupLabel { get; private set; }

    [field: SerializeField]
    public Interaction[] CurrentInteractions { get; private set; }
}