using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainOperator : MonoBehaviour
{
    public Transform train;
    public TrainTrack track;
    public float speed;

    private int currentStationIdx = 0;
    private int nextStationIdx;
    private float distanceToNextStation;
    private float distanceRemaining;

    private void Start()
    {
        train.position = track.stations[currentStationIdx].transform.position;
        distanceToNextStation = track.distanceToNextStation[currentStationIdx];
        Debug.Log(distanceToNextStation.ToString());
        nextStationIdx = track.GetLoopIdx(currentStationIdx + 1);
    }

    private void Update()
    {

        if (distanceRemaining <= 0)
        {
            currentStationIdx = nextStationIdx;
            nextStationIdx = track.GetLoopIdx(currentStationIdx + 1);
            distanceToNextStation = track.distanceToNextStation[currentStationIdx];
            distanceRemaining = distanceToNextStation;
        }
        else
        {
            distanceRemaining -= speed * Time.deltaTime;
            float t = (distanceToNextStation - distanceRemaining) / distanceToNextStation;

            train.position = Bezier.EvaluateCubic(track.stations[currentStationIdx].transform.position,
                                                  track.stations[currentStationIdx].postControl.transform.position,
                                                  track.stations[nextStationIdx].preControl.transform.position,
                                                  track.stations[nextStationIdx].transform.position, 
                                                  t);
            
            train.forward = Bezier.TangentCubic(track.stations[currentStationIdx].transform.position,
                                                  track.stations[currentStationIdx].postControl.transform.position,
                                                  track.stations[nextStationIdx].preControl.transform.position,
                                                  track.stations[nextStationIdx].transform.position,
                                                  t);

            
        }
    }

}
