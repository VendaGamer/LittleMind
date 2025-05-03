using UnityEngine;

/// <summary>
/// Detekuje zda je objekt videt v kamere
/// </summary>
public class CameraVisibilityDetector : MonoBehaviour
{
    private new Collider collider;
    private new Renderer renderer;
    private void Start()
    {
        collider = GetComponent<Collider>();
        renderer = GetComponent<Renderer>();
    }
    private void LateUpdate()
    {
        renderer.material.color = 
        GeometryUtility.TestPlanesAABB(PlayerCamera.Instance?.FrustumPlanes, collider.bounds)
            ? Color.green
            : Color.red;
    }
}