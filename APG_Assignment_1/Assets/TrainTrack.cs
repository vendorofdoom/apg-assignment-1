using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Place stations
// Position control points
// Generate track points
// Add lil train to move around between stations
// Add track mesh?
// Get lil train to point in the right direction around track

// Parts of this script are adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

public class TrainTrack : MonoBehaviour
{
    [Header("Stations")]
    public int numStations;
    public List<Station> stations;
    public float[] distanceToNextStation;
    public Transform stationParent;
    public GameObject stationPrefab;

    [Header("Track")]
    public float spacing = 0.1f;
    public float resolution = 1f;
    public Vector3[] trackPoints;
    public Vector3[] trackDirs;
    public Transform trackParent;
    public GameObject trackPrefab;


    private void Awake()
    {
        PlaceStations();
        SetAllStationControlPoints();
        GenerateTrackPoints();
        DrawTrack();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ClearTracksAndStations();
            PlaceStations();
            SetAllStationControlPoints();
            GenerateTrackPoints();
            DrawTrack();
        }
    }


    private void ClearTracksAndStations()
    {
        foreach(Transform child in trackParent)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (Transform child in stationParent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void PlaceStations()
    {
        stations = new List<Station>();

        Vector3 origin = Vector3.zero;
        Vector3 dir = Vector3.forward;

        for (int i = 0; i < numStations; i++)
        {
            GameObject g = GameObject.Instantiate(stationPrefab, stationParent);
            g.transform.position = (origin + dir * Random.Range(10f, 20f)) + (Vector3.up * Random.Range(0f, 10f));
            stations.Add(g.GetComponent<Station>());
            dir = Quaternion.Euler(0, 360f / numStations, 0) * dir;
        }

    }

    private void DrawTrack()
    {
        for (int i = 0; i < trackPoints.Length; i++)
        {
            GameObject g = GameObject.Instantiate(trackPrefab, trackParent);
            g.transform.position = trackPoints[i];
            g.transform.localScale *= (spacing * 2f);
            g.transform.forward = trackDirs[i];
        }
    }

    // Calc distance between stations and evenly spaced distances / forwards between them
    private void GenerateTrackPoints()
    {
        distanceToNextStation = new float[stations.Count];

        if (stations.Count < 2)
        {
            return; // not enough to form a loop!
        }

        List<Vector3> evenlySpacedPoints = new List<Vector3>();
        evenlySpacedPoints.Add(stations[0].transform.position);
        Vector3 previousPoint = stations[0].transform.position;
        float distanceSinceLastEvenPoint = 0;

        List<Vector3> evenlySpacedForwards = new List<Vector3>();
        evenlySpacedForwards.Add((stations[0].postControl.transform.position - stations[0].transform.position).normalized); // vector looking at the next control point

        for (int i = 0; i < stations.Count; i++)
            {
                int nextIdx = GetLoopIdx(i+1);

                Vector3[] p = new Vector3[] { stations[i].transform.position, 
                                              stations[i].postControl.transform.position,
                                              stations[nextIdx].preControl.transform.position,
                                              stations[nextIdx].transform.position};

                float controlNetLength = Vector3.Distance(p[0], p[1]) + Vector3.Distance(p[1], p[2]) + Vector3.Distance(p[2], p[3]);
                float estimatedCurveLength = Vector3.Distance(p[0], p[3]) + (controlNetLength * 0.5f);
                distanceToNextStation[i] = estimatedCurveLength;
                int divisions = Mathf.CeilToInt(estimatedCurveLength * resolution * 10); // some const 10 

                float t = 0;
                while (t <= 1)
                {
                    t += 1f / divisions;
                    Vector3 pointOnCurve = Bezier.EvaluateCubic(p[0], p[1], p[2], p[3], t);
                    distanceSinceLastEvenPoint += Vector3.Distance(previousPoint, pointOnCurve);

                    while (distanceSinceLastEvenPoint >= spacing)
                    {
                        float overshootDist = distanceSinceLastEvenPoint - spacing;
                        Vector3 newEvenlySpacedPoint = pointOnCurve + (previousPoint - pointOnCurve).normalized * overshootDist;
                        evenlySpacedPoints.Add(newEvenlySpacedPoint);
                        evenlySpacedForwards.Add((pointOnCurve - previousPoint).normalized);
                        distanceSinceLastEvenPoint = overshootDist;
                        previousPoint = newEvenlySpacedPoint;
                    }
                    previousPoint = pointOnCurve;
                }
            }

            trackPoints = evenlySpacedPoints.ToArray();
            trackDirs = evenlySpacedForwards.ToArray();
        
        }

    public int GetLoopIdx(int i)
    {
        return (i + stations.Count) % stations.Count;
    }

    void SetAllStationControlPoints()
    {
        for (int i = 0; i < stations.Count; i++)
        {
            SetStationControlPoints(i);
        }

    }

    void SetStationControlPoints(int stationIdx)
    {

        Vector3 currStation = stations[stationIdx].transform.position;
        Vector3 dir = Vector3.zero;

        Vector3 prevOffset = stations[GetLoopIdx(stationIdx - 1)].transform.position - currStation;
        Vector3 postOffset = stations[GetLoopIdx(stationIdx + 1)].transform.position - currStation;

        dir = prevOffset - postOffset;
        dir.Normalize();

        stations[stationIdx].transform.forward = dir * -0.5f;

        stations[stationIdx].preControl.transform.position = currStation + dir * prevOffset.magnitude * 0.5f;
        stations[stationIdx].postControl.transform.position = currStation + dir * -postOffset.magnitude * 0.5f;

        
    }
}




