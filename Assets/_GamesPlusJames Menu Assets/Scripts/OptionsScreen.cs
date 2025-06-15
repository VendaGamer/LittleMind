using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Search.Providers;

public class Options : MonoBehaviour
{
    public Toggle fullscreenTog, vsyncToq;

    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedResolution;
    public TMP_Text resolutionLabel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fullscreenTog.isOn = Screen.fullScreen;

        if(QualitySettings.vSyncCount == 0)
        {
            vsyncToq.isOn = false;
        }
        else
        {
            vsyncToq.isOn = true;
        }

        bool foundRes = false;
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].horizontal == Screen.currentResolution.width && resolutions[i].vertical == Screen.currentResolution.height)
            {
                foundRes = true;
                selectedResolution = i;
                UpdateResLabel();
            }
        }

        if(!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.currentResolution.width;
            newRes.vertical = Screen.currentResolution.height;

            resolutions.Add(newRes);
            selectedResolution = resolutions.Count - 1;

            UpdateResLabel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResLeft()
    {
        selectedResolution--;
        if(selectedResolution < 0)
        {
            selectedResolution = resolutions.Count - 1;
        }

        UpdateResLabel();
    }

    public void ResRight()
    {
        selectedResolution++;
        if (selectedResolution > resolutions.Count - 1)
        {
            selectedResolution = 0;
        }

        UpdateResLabel();
    }

    public void UpdateResLabel()
    {
        resolutionLabel.text = resolutions[selectedResolution].horizontal.ToString() + " x " + resolutions[selectedResolution].vertical.ToString();
    }

    public void ApplyGraphics()
    {
        Screen.fullScreen = fullscreenTog.isOn;
        if (vsyncToq.isOn)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 300;
        }

        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullscreenTog.isOn);
    }
}

[System.Serializable]
public class ResItem
{
    public int horizontal, vertical;
}
