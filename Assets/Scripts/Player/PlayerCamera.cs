using System;
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Singleton kamery hráče který každý frame kalkuluje Frustumy
/// Stara se taky o prepinani mezi mody kamery
/// </summary>
[DefaultExecutionOrder(-10)]
public class PlayerCamera : MonoBehaviourSingleton<PlayerCamera>
{
    [SerializeField]
    private CinemachineCamera PlayerCinCamera;

    [SerializeField]
    private float maxDistance = 0.05f;

    [SerializeField]
    private LayerMask collisionMask;
    public Camera Camera { get; private set; }

    public float PlayerCameraFOV
    {
        get => PlayerCinCamera.Lens.FieldOfView;
        set => PlayerCinCamera.Lens.FieldOfView = value;
    }

    public Plane[] FrustumPlanes { get; } = new Plane[6];
    private float frustumExpansionFactor = 1.1f;
    public event Action OnBlendFinished;

    public int CurrentVirtualCameraPriority
    {
        get
        {
            if (cinemachineBrain.ActiveVirtualCamera is CinemachineVirtualCameraBase virtualCamera)
            {
                return virtualCamera.Priority;
            }

            return -1;
        }
    }

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
    private bool wasBlending = false;

    protected override void Awake()
    {
        base.Awake();
        Camera = Camera.main;
        cinemachineBrain = GetComponent<CinemachineBrain>();
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

    private void LateUpdate()
    {
        UpdateFrustum();
    }

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
