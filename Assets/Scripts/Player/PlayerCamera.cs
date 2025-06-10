using System;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Singleton kamery hráče který každý frame kalkuluje Frustumy
/// Stara se taky o prepinani mezi mody kamery
/// </summary>
///
[DefaultExecutionOrder(10)]
public class PlayerCamera : MonoBehaviourSingleton<PlayerCamera>
{
    public Camera Camera { get; private set; }

    public Plane[] FrustumPlanes { get; } = new Plane[6];
    private float frustumExpansionFactor = 1.1f;
    public event Action OnBlendFinished;
    public int CameraPriority => cinemachineCamera.Priority.Value;

    public float FrustumExpansionFactor
    {
        get => frustumExpansionFactor;
        set
        {
            frustumExpansionFactor = value;
            UpdateFrustum();
        }
    }

    private CinemachineBrain cinemachineBrain;
    private CinemachineCamera cinemachineCamera;
    private bool wasBlending = false;

    protected override void Awake()
    {
        base.Awake();
        Camera = GetComponentInChildren<Camera>();
        cinemachineBrain = GetComponentInChildren<CinemachineBrain>();
        cinemachineCamera = GetComponentInChildren<CinemachineCamera>();
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
}
