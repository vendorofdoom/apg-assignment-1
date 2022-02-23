using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainOperator : MonoBehaviour
{
    public Transform carriage;
    public TrainTrack track;
    public float speed;

    private int currentStationIdx = 0;
    private int nextStationIdx;
    private float distanceToNextStation;
    private float distanceRemaining;

    public Transform nextCarriage;
    public bool moving = false;
    public float distToNextCarriage = 1f;
    public float totalDistTravelled = 0f;

    private void Start()
    {
        carriage.position = track.stations[currentStationIdx].transform.position;
        distanceToNextStation = track.distanceToNextStation[currentStationIdx];
        nextStationIdx = track.GetLoopIdx(currentStationIdx + 1);
    }

    private void Update()
    {
        if (!moving)
        {
            return;
        }

        totalDistTravelled += speed * Time.deltaTime;
        if (totalDistTravelled >= distToNextCarriage && nextCarriage != null && !nextCarriage.GetComponent<TrainOperator>().moving)
        {
            nextCarriage.GetComponent<TrainOperator>().moving = true;
        }

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

            carriage.position = Bezier.EvaluateCubic(track.stations[currentStationIdx].transform.position,
                                                  track.stations[currentStationIdx].postControl.transform.position,
                                                  track.stations[nextStationIdx].preControl.transform.position,
                                                  track.stations[nextStationIdx].transform.position, 
                                                  t);

            carriage.forward = Bezier.TangentCubic(track.stations[currentStationIdx].transform.position,
                                                  track.stations[currentStationIdx].postControl.transform.position,
                                                  track.stations[nextStationIdx].preControl.transform.position,
                                                  track.stations[nextStationIdx].transform.position,
                                                  t);

            
        }

    }

}
