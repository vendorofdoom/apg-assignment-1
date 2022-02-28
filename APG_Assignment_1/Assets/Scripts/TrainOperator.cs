using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Move service stuff out into it's own config?

public class TrainOperator : MonoBehaviour
{
    public TrainTrack track;
    public TrainService trainService;

    public GameObject locomotivePrefab;
    public GameObject[] carriagePrefabs;

    [Header("Camera")]
    public CameraFollow camFollow;
    public CameraController camController;

    public Slider speedSlider;
    public TMPro.TextMeshProUGUI announcements;

    private int numCarriages;
    private float carriageSpacing = 2f;
    private Transform[] carriages;
    private float[] t;
    private float speed;

    public void SpeedSliderChange()
    {
        speed = speedSlider.value;
    }

    public void LocalService()
    {
        UpdateService(TrainService.ServiceType.Local);
    }

    public void NationalService()
    {
        UpdateService(TrainService.ServiceType.National);
    }

    public void SleeperService()
    {
        UpdateService(TrainService.ServiceType.Sleeper);
    }

    private void UpdateService(TrainService.ServiceType tService)
    {
        DestroyTrain();
        trainService.Service = tService;
        SetupTrain();
    }


    private void Start()
    {
        speed = speedSlider.value;
        SetupTrain();
    }

    private void SetupTrain()
    {
        numCarriages = Random.Range(trainService.minCarriages, trainService.maxCarriages);
        Debug.Log(numCarriages);
        carriages = new Transform[numCarriages];
        t = new float[numCarriages];

        float startT = 0f;

        for (int i = numCarriages - 1; i >= 0; i--)
        {
            // set up the prefab for the carriage
            GameObject carriageObj;
            if (i == 0)
            {
                carriageObj = GameObject.Instantiate(locomotivePrefab, transform);

                camController.trainCam = carriageObj.GetComponentInChildren<Camera>();
                if (camController.CamSetting != CameraController.CameraSetting.Train)
                {
                    camController.trainCam.enabled = false;
                }

            }
            else
            {
                carriageObj = GameObject.Instantiate(carriagePrefabs[Random.Range(0, carriagePrefabs.Length)], transform);
            }

            carriages[i] = carriageObj.GetComponent<Transform>();

            t[i] = startT;

            carriages[i].position = track.bezierLoop.AnchorPoint(0);

            startT += carriageSpacing;
        }

        camFollow.target = carriages[0];
        AnnounceNextStop(-1); // assume we start at the first station?
    }

    private void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            DestroyTrain();
            SetupTrain();
        }

        for (int i = 0; i < numCarriages; i++)
        {
            t[i] += Time.deltaTime * speed;

            Vector3 position;
            Vector3 forward;
            track.bezierLoop.PosAndForwardForDistance(t[i], out position, out forward);

            carriages[i].position = position;
            carriages[i].forward = forward;
        }

        // Make announcement about next station
        if (track.StationAtDistance(t[0]) > -1)
        {
            AnnounceNextStop(track.StationAtDistance(t[0]));
        }
    }

    private void AnnounceNextStop(int currStationIdx)
    { 
        if (track.stations.Count > 0)
        {
            int nextStationIdx = (currStationIdx + 1 + track.stations.Count) % track.stations.Count;
            announcements.text = "Next stop: " + track.stations[nextStationIdx].stationName;
        }
        
    }

    private void DestroyTrain()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
