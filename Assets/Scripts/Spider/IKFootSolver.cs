using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using JetBrains.Annotations;
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
    
    private void Update()
    {
        var change = Vector3.Distance(transform.position, currentPos);
        transform.position = currentPos;
        if (Vector3.Distance(transform.position, legMoveTarget.position) > legMoveThreshold)
        {
            currentPos = legMoveTarget.position;
            shouldMove = true;
        }

        if (Physics.Raycast(legMoveTarget.position + (0.3f* Vector3.up), Vector3.down, out var hit, data.MaxRaycastDistance,
                data.TerrainLayer))
        {
            legMoveTarget.position = hit.point;
        }
    }

    [CanBeNull]
    public TweenerCore<Vector3,Vector3,VectorOptions> MoveToMoveTarget()
    {
        return transform.position == legMoveTarget.position ? null
            : transform.DOMove(legMoveTarget.position,1f);
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