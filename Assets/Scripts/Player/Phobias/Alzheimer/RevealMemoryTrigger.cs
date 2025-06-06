using UnityEngine;

/// <summary>
/// This memory trigger reveals gameobjects to player when discovered
/// </summary>
public class RevealMemoryTrigger : MemoryTrigger
{
    [SerializeField]
    private GameObject[] objectsToReveal;

    [SerializeField] private Transform PointToLookAt;

    public override Bounds BoundsToLookAt => BoundsOfPointToLookAt;
    
    private Bounds BoundsOfPointToLookAt;

    private void Start()
    {
        BoundsOfPointToLookAt = new Bounds(PointToLookAt.position, Vector3.zero);
    }

    public override void MemoryDiscovered()
    {
        foreach (var obj in objectsToReveal)
        {
            obj.SetActive(true);
        }
    }
}