using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainOperator : MonoBehaviour
{
    public TrainTrack track;
    public float speed;

    public GameObject locomotivePrefab;
    public GameObject carriagePrefab;

    [Range(1, 8)]
    public int numCarriages = 1;

    [SerializeField]
    private Transform[] carriages;
    private int[] fromStationIdx;
    private int[] toStationIdx;
    private float[] totalDist;
    private float[] t;

    private void Start()
    {
        SetupTrain();
    }


    private void SetupTrain()
    {
        carriages = new Transform[numCarriages];
        fromStationIdx = new int[numCarriages];
        toStationIdx = new int[numCarriages];
        totalDist = new float[numCarriages];
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
                carriageObj = GameObject.Instantiate(carriagePrefab, transform);
            }
            carriages[i] = carriageObj.GetComponent<Transform>();

            // which station is the carriage inbetween?
            fromStationIdx[i] = 0;
            toStationIdx[i] = track.GetLoopIdx(fromStationIdx[i] + 1);
            totalDist[i] = track.distanceToNextStation[fromStationIdx[i]];
            t[i] = startT;

            // place the game object
            carriages[i].position = track.stations[fromStationIdx[i]].transform.position;

            startT += 0.1f; // TODO: find a better way to space carriages? maybe calc distance? :/ just hard-coded for now...
        }


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
            if (t[i] >= 1)
            {
                fromStationIdx[i] = toStationIdx[i];
                toStationIdx[i] = track.GetLoopIdx(fromStationIdx[i] + 1);
                totalDist[i] = track.distanceToNextStation[fromStationIdx[i]];
                t[i] = 0f;
            }
            else
            {
                t[i] += (speed * Time.deltaTime) / totalDist[i];

                carriages[i].position = Bezier.EvaluateCubic(track.stations[fromStationIdx[i]].transform.position,
                                                      track.stations[fromStationIdx[i]].postControl.transform.position,
                                                      track.stations[toStationIdx[i]].preControl.transform.position,
                                                      track.stations[toStationIdx[i]].transform.position,
                                                      t[i]);

                carriages[i].forward = Bezier.TangentCubic(track.stations[fromStationIdx[i]].transform.position,
                                                      track.stations[fromStationIdx[i]].postControl.transform.position,
                                                      track.stations[toStationIdx[i]].preControl.transform.position,
                                                      track.stations[toStationIdx[i]].transform.position,
                                                      t[i]);


            }
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
