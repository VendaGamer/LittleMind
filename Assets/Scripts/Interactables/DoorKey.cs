public class DoorKey : GeneralPickableObject
{
    protected override void OnPicked(IInteractor interactor)
    {
        interactor.SetHandTarget(IKTargetType.key);
    }
}
