using System.Collections;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] private IKFootSolverData data;
    [SerializeField] private Transform legMoveTarget;
    private bool shouldMove;
    [SerializeField] private float legMoveThreshold = 0.2f;
    private Vector3 currentPos;
    private Vector3 basePos;
    private Vector3 newPosition;

    private void Start()
    {
        currentPos = transform.position;
    }
    
    void Update()
    {
        var change = Vector3.Distance(transform.position, currentPos);
        transform.position = currentPos;
        if (Vector3.Distance(transform.position, legMoveTarget.position) > legMoveThreshold)
        {
            currentPos = legMoveTarget.position;
            shouldMove = true;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(legMoveTarget.position, 0.01f);
        var dist = Vector3.Distance(transform.position, legMoveTarget.position);
        if (dist > 0.01f)
        {
            Gizmos.color = Color.Lerp(Color.green, Color.red, dist / legMoveThreshold);
            Gizmos.DrawRay(transform.position, legMoveTarget.position - transform.position);
        }
    }
}