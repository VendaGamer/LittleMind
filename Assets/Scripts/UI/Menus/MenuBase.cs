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

    public void Show() => uiCamera.Priority = 10;
    
    public void Hide() => uiCamera.Priority = 0;

}