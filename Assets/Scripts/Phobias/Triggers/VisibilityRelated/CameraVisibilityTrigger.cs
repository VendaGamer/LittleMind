using UnityEngine;

/// <summary>
/// Detekuje zda je objekt videt v kamere
/// </summary>
public abstract class CameraVisibilityTrigger : MonoBehaviour
{
    private bool isInPlayersView = true;

    protected bool IsInPlayersView
    {
        get => isInPlayersView;
        set
        {
            isInPlayersView = value;
            OnIsInPlayersViewChanged();
        }
    }

    protected abstract void OnIsInPlayersViewChanged();

    private Collider col;

    protected virtual void Start()
    {
        col = GetComponent<Collider>();
    }

    protected virtual void LateUpdate()
    {
        IsInPlayersView = GeometryUtility.TestPlanesAABB(
            PlayerCamera.Instance.FrustumPlanes,
            col.bounds
        );
    }
}
