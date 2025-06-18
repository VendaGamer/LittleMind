using System;
using Unity.Cinemachine;
using UnityEngine;

[Serializable]
public class MenuBase
{
    [Header("Root object of menu should be panel")]
    [SerializeField]
    private GameObject root;
    
    [Header("Cinemachine camera")]
    [SerializeField]
    private CinemachineCamera uiCamera;
    public void SetCameraPriority(int priority) => uiCamera.Priority = priority;

    public void SetActive(bool value) => root.SetActive(value);

}