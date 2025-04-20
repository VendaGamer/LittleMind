using UnityEngine;

public class SpiderMoveController : MonoBehaviour
{
    [SerializeField] private float bodyHeight = 0.3f;
    private IKFootSolver[] footSolvers;

    private void Start()
    {
        footSolvers = GetComponentsInChildren<IKFootSolver>();
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out var hit, bodyHeight))
        {
            transform.position += transform.TransformDirection(Vector3.up) * hit.distance;
        }
    }

    private void OnVelocityZero()
    {
        foreach (var solver in footSolvers)
        {
            solver.MoveToMoveTarget();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * bodyHeight);
    }
}