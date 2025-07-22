using System;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
public partial class GlobalInteractionGroup : IInteractionGroup
{
    [SerializeField, DontCreateProperty] 
    private string interactGroupLabel;
    
    [CreateProperty]
    public string InteractGroupLabel => interactGroupLabel;

    [SerializeField, DontCreateProperty]
    public Interaction[] currentInteractions;
    
    [CreateProperty]
    public Interaction[] CurrentInteractions => currentInteractions;
}