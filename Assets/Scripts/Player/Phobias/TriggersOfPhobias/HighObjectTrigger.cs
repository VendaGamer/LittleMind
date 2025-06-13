using UnityEngine;

public class HighObjectTrigger : ObservableAnxietyTrigger<Acrofobia>
{
    protected override void OnPlayerWithAnxietyStay(Acrofobia other)
    {
        var playerPosition = other.transform.position;
        var dist = GetNearestEdgeDistance(playerPosition);
        other.IncreaseAnxiety(0.5f * GetNearestEdgeDistance(playerPosition));
    }

    private float GetNearestEdgeDistance(Vector3 playerPosition)
    {
        return Mathf.Min(
            Mathf.Abs(playerPosition.x - transform.position.x - transform.localScale.x /2),
            Mathf.Abs(playerPosition.z - transform.position.z - transform.localScale.z /2),
            Mathf.Abs(playerPosition.z - transform.position.z + transform.localScale.z /2),
            Mathf.Abs(playerPosition.x - transform.position.x + transform.localScale.x /2)
        );
    }


}
