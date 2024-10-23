using UnityEngine;

public class PlayerController : FPSController
{
    [Header("Illness")]
    [SerializeField]
    private MentalIllness currentIllness;
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
