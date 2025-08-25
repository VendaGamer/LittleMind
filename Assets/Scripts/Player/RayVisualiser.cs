using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RayVisualizer : MonoBehaviour
{
    LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = 0.02f;
        lr.endWidth = 0.02f;
        lr.material = new Material(Shader.Find("Sprites/Default")); 
        lr.startColor = Color.red;
        lr.endColor = Color.red;
    }

    void LateUpdate()
    {
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        lr.SetPosition(0, ray.origin);
        lr.SetPosition(1, ray.origin + ray.direction * 100f);
    }
}