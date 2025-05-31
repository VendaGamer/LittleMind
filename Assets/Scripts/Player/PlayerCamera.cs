using System;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Singleton kamery hráče který každý frame kalkuluje Frustumy
/// </summary>
public class PlayerCamera : MonoBehaviourSingleton<PlayerCamera>
{
    public Camera Camera { get; private set; }
    public Plane[] FrustumPlanes { get; } = new Plane[6];
    private float frustumExpansionFactor = 1.1f;

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

    private void Start()
    {
        Camera = GetComponent<Camera>();
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
    
}
