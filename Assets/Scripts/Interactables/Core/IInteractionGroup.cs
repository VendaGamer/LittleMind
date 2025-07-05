using System.Collections.Generic;

public interface IInteractionGroup
{
    public string InteractGroupLabel { get; }
    public IReadOnlyList<Interaction> CurrentInteractions { get; }

    public void RebuildKeys()
    {
        foreach (var interaction in CurrentInteractions)
        {
            interaction.RebuildKey();
        }
    }
}