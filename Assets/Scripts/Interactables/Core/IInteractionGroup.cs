using System.Collections.ObjectModel;
using DG.Tweening;

public interface IInteractionGroup
{
    public string InteractGroupLabel { get; }
    public Interaction[] CurrentInteractions { get; }

    public void RebuildKeys()
    {
        foreach (var interaction in CurrentInteractions)
        {
            interaction.RebuildKey();
        }
    }
}