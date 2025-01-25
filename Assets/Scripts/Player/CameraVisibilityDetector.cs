using UnityEngine;

/// <summary>
/// Detekuje zda je objekt videt v kamere
/// </summary>
public class CameraVisibilityDetector : MonoBehaviour
{
    private Collider collider;
    private Renderer renderer;
    private void Start()
    {
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();
    }
    private void LateUpdate()
    {
        renderer.material.color = 
        GeometryUtility.TestPlanesAABB(PlayerCamera.FrustumPlanes, collider.bounds)
            ? Color.green
            : Color.red;
    }
}