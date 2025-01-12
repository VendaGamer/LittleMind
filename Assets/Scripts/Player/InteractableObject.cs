using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InteractableObject : MonoBehaviour
{
    [SerializeField]
    protected string displayName;
    public abstract HintData[] GetContextualHints();
    
    public abstract void Interact(InputActionReference actionReference);

}