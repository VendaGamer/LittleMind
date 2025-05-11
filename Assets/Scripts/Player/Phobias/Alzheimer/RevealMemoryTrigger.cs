using UnityEngine;

/// <summary>
/// This memory trigger reveals gameobjects to player when discovered
/// </summary>
public class RevealMemoryTrigger : MemoryTrigger
{
    [SerializeField]
    private GameObject[] objectsToReveal;
    
    public override void MemoryDiscovered()
    {
        foreach (var obj in objectsToReveal)
        {
            obj.SetActive(true);
        }
    }
}