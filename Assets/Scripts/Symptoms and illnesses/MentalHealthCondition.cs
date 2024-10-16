using UnityEngine;

public abstract class MentalHealthCondition : MonoBehaviour
{
    public string Name { get; protected set; }
    public float Severity { get; protected set; }
    public abstract void ApplyEffect(GameObject player);
}
