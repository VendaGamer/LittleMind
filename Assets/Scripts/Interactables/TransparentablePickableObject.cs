using UnityEngine;

public class TransparentablePickableObject : PickableObject
{
    [SerializeField]
    protected TransparentablePickableObjectData data;
    protected override PickableObjectData Data => data;
    private float originalTransparency;

    private Renderer rend;

    protected override void Start()
    {
        base.Start();
        rend = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();
        originalTransparency = rend.material.color.a;
    }

    protected override void OnPicked()
    {
        var col = rend.material.color;
        
        rend.material.color = new Color(
        col.r, col.g, col.b,
        Mathf.Lerp(originalTransparency, 0f, data.TransparencyPercentage)
        );
    }

    protected override void OnDropped()
    {
        var col = rend.material.color;
        rend.material.color = new Color(col.r, col.g, col.b, originalTransparency);
    }
}