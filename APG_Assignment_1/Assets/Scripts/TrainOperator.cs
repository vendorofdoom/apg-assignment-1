using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: Move service stuff out into it's own config?

public class TrainOperator : MonoBehaviour
{
    public TrainTrack track;

    public TrainService service;

    private float speed;
    public Slider speedSlider;

    public GameObject locomotivePrefab;
    public GameObject[] carriagePrefabs;
    private int numCarriages;
    private float carriageSpacing = 2f;
    private Transform[] carriages;

    public TMPro.TextMeshProUGUI announcements;

    public CameraController camControl;

    private float[] t;



    public void SpeedSliderChange()
    {
        speed = speedSlider.value;
    }

    public void LocalService()
    {
        UpdateService(TrainService.Local);
    }

    public void NationalService()
    {
        UpdateService(TrainService.National);
    }

    public void SleeperService()
    {
        UpdateService(TrainService.Sleeper);
    }

    private void UpdateService(TrainService tService)
    {
        DestroyTrain();
        service = tService;
        SetupTrain();
    }

    public enum TrainService
    {
        Local,
        National,
        Sleeper
    }

    private int CarriageMin()
    {
        switch (service)
        {
            case TrainService.Local:
                return 3;
            case TrainService.National:
                return 6;
            case TrainService.Sleeper:
                return 11;
            default:
                return 1;
        }
    }

    private int CarriageMax()
    {
        switch (service) {
            case TrainService.Local:
                return 5;
            case TrainService.National:
                return 10;
            case TrainService.Sleeper:
                return 20;
            default:
                return 10;
        }
    }


    private void Start()
    {
        speed = speedSlider.value;
        SetupTrain();
    }

    private void SetupTrain()
    {
        numCarriages = Random.Range(CarriageMin(), CarriageMax());

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
            }
            else
            {
                carriageObj = GameObject.Instantiate(carriagePrefabs[Random.Range(0, carriagePrefabs.Length)], transform);
            }

            carriages[i] = carriageObj.GetComponent<Transform>();

            t[i] = startT;

            // place the game object
            carriages[i].position = track.bezierLoop.AnchorPoint(0);

            startT += carriageSpacing;
        }

        camControl.target = carriages[0];
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
        int nextStationIdx = (currStationIdx + 1 + track.stations.Count) % track.stations.Count;
        announcements.text = "Next stop: " + track.stations[nextStationIdx].stationName;
    }

    private void DestroyTrain()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
