using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CameraSetting camSetting;

    public Camera mainCam;
    public Camera trainCam;
    public CameraFollow camFollow;

    public enum CameraSetting
    {
        Train,
        Main,
        MainFollow
    }

    public CameraSetting CamSetting
    {
        get
        {
            return camSetting;
        }
        set
        {
            camSetting = value;
            CameraSetup();
        }
    }

    private void Start()
    {
        camSetting = CameraSetting.Main;
        CameraSetup();
    }

    private void CameraSetup()
    {
        switch (camSetting)
        {
            case CameraSetting.Main:
                trainCam.enabled = false;
                mainCam.enabled = true;
                camFollow.enabled = false;
                break;

            case CameraSetting.MainFollow:
                trainCam.enabled = false;
                mainCam.enabled = true;
                camFollow.enabled = true;
                break;

            case CameraSetting.Train:
                mainCam.enabled = false;
                trainCam.enabled = true;
                break;

        }
    }

    public void CameraToggle()
    {

        switch (camSetting)
        {
            case CameraSetting.Main:
                CamSetting = CameraSetting.MainFollow;
                break;

            case CameraSetting.MainFollow:
                CamSetting = CameraSetting.Train;
                break;

            case CameraSetting.Train:
                CamSetting = CameraSetting.Main;
                break;
        }
    }


}
