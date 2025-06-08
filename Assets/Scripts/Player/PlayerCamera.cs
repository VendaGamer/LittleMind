using System;
using Cinemachine;
using UnityEngine;
/// <summary>
/// Singleton kamery hráče který každý frame kalkuluje Frustumy
/// Stara se taky o prepinani mezi mody kamery
/// </summary>
///
[DefaultExecutionOrder(-2000)]
public class PlayerCamera : MonoBehaviourSingleton<PlayerCamera>
{
    public Camera Camera { get; private set; }
    
    public Plane[] FrustumPlanes { get; } = new Plane[6];
    private float frustumExpansionFactor = 1.1f;
    public event Action OnBlendFinished;

    public float FrustumExpansionFactor
    {
        get => frustumExpansionFactor;
        set
        {
            frustumExpansionFactor = value;
            UpdateFrustum();
        }
    }

    private Vector3 lastCameraPosition;
    private Quaternion lastCameraRotation;
    private CinemachineBrain cinemachineBrain;
    private bool wasBlending = false;

    private void Start()
    {
        Camera = GetComponentInChildren<Camera>();
        cinemachineBrain = GetComponentInChildren<CinemachineBrain>();
    }

    private void Update()
    {
        if (cinemachineBrain.IsBlending)
        {
            wasBlending = true;
        }
        else if (wasBlending)
        {
            wasBlending = false;
            OnBlendFinished?.Invoke();
        }
    }

    private void LateUpdate() => UpdateFrustum();

    private void UpdateFrustum()
    {
        lastCameraPosition = transform.position;
        lastCameraRotation = transform.rotation;

        // Copy and expand projection matrix
        var projection = Matrix4x4.Perspective(
            Camera.fieldOfView * FrustumExpansionFactor,
            Camera.aspect,
            Camera.nearClipPlane,
            Camera.farClipPlane
        );

        var worldToCamera = Camera.worldToCameraMatrix;
        var vp = projection * worldToCamera;

        GeometryUtility.CalculateFrustumPlanes(vp, FrustumPlanes);
    }

    public void StopAim()
    {
        var brain = CinemachineCore.Instance.GetActiveBrain(0);
        
    }
    
}
