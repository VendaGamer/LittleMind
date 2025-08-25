using UnityEngine;

/// <summary>
/// This memory trigger reveals gameobjects to player when discovered
/// </summary>
public class RevealMemoryTrigger : MemoryTrigger
{
    [SerializeField]
    private GameObject[] objectsToReveal;

    public int indexOfNote;
    

    public override void MemoryDiscovered()
    {
        foreach (var obj in objectsToReveal)
        {
            obj.SetActive(true);
        }
    }
}