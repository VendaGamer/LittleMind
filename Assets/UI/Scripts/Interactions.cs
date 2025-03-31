using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Interactions/Interactions",fileName = "Interactions")]
public class Interactions : ScriptableObject, ISingleton
{
    [SerializeField] private List<UIInteractionGroup> uiInteractionGroups;
    [SerializeField] private bool crosshairVisible = true;
    
    [CreateProperty]
    public bool CrosshairVisible =>crosshairVisible;
    [CreateProperty]
    public List<UIInteractionGroup> UIInteractionGroups =>uiInteractionGroups;
    
}