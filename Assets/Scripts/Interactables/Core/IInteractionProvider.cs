using System;
using System.Collections.Generic;
using UnityEngine.InputSystem.Utilities;

public interface IInteractionProvider
{
    IReadOnlyList<Interaction> CurrentInteractions { get; }
    
    event Action InteractionsChanged;
}