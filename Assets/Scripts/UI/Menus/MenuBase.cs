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

    public void Show() => uiCamera.Prioritize();
    
    public void Hide() => uiCamera.Priority = -1;

    public void SetActive(bool value) => root.SetActive(value);

}