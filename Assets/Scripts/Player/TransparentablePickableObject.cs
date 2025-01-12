using UnityEngine;

public class TransparentablePickableObject : PickableObject
{
    [SerializeField] private float transparencyOnPick;
    private float originalTransparency;

    private Renderer rend;

    protected override void Start()
    {
        base.Start();
        rend = GetComponent<Renderer>();
        originalTransparency = rend.material.color.a;
    }

    protected override void OnPicked()
    {
        var col = rend.material.color;
        rend.material.color = new Color(col.r, col.g, col.b, transparencyOnPick);
    }

    protected override void OnDropped()
    {
        var col = rend.material.color;
        rend.material.color = new Color(col.r, col.g, col.b, originalTransparency);
    }
}