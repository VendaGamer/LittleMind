using UnityEngine;

public class SpiderMoveController : MonoBehaviour
{
    [SerializeField] private float bodyHeight = 0.3f;
    private IKFootSolver[] footSolvers;
    private Rigidbody rb;

    private void Start()
    {
        footSolvers = GetComponentsInChildren<IKFootSolver>();
        rb = GetComponentInParent<Rigidbody>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out var hit, bodyHeight))
        {
            transform.position += transform.TransformDirection(Vector3.up) * hit.distance;
        }

        if (Mathf.Approximately(rb.linearVelocity.magnitude, 0f))
        {
            OnVelocityZero();
        }
    }

    private void OnVelocityZero()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * bodyHeight);
    }
}