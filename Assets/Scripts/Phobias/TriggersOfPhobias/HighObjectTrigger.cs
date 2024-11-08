using UnityEngine;

public class HighObjectTrigger : MonoBehaviour
{
    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.TryGetComponent<Akrofobia>(out var akrofobia))
        {
            Vector3 playerPosition = other.transform.position;
            var dist = GetNearestEdgeDistance(playerPosition);
            akrofobia.SetAnxietyBasedOnDistance(dist);
            Debug.Log("distance: "+dist);
        }
    }

    private float GetNearestEdgeDistance(Vector3 playerPosition)
    {
        return Mathf.Min(
            playerPosition.x - transform.position.x - transform.localScale.x /2,
            playerPosition.z - transform.position.z - transform.localScale.z /2,
            playerPosition.z - transform.position.z + transform.localScale.z /2,
            playerPosition.x - transform.position.x + transform.localScale.x /2);
    }
}
