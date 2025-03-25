using UnityEngine;
public class HighObjectTrigger : MonoBehaviour
{
    private void OnCollisionStay(Collision other)
    {
        if (other.collider.TryGetComponent<Akrofobia>(out var akrofobia))
        {
            Vector3 playerPosition = other.transform.position;
            var dist = GetNearestEdgeDistance(playerPosition);
            akrofobia.IncreaseAnxiety(0.5f * GetNearestEdgeDistance(playerPosition));
        }
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
