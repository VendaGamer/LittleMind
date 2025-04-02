public class DoorTrigger : CameraVisibilityTrigger
{
    private Door door;

    protected override void Start()
    {
        base.Start();
        door = GetComponent<Door>();
    }

    protected override void OnIsInPlayersViewChanged()
    {
        if (!IsInPlayersView)
        {
            door.OpenDoor();
        }
        else
        {
            door.CloseDoor();
        }
    }
}
