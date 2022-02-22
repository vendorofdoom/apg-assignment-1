using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainTrack : MonoBehaviour
{
    public Station[] stations;

    public float spacing = 0.1f;
    public float resolution = 1f;

    public Vector3[] trackPoints;

    public Transform train;


    private void Start()
    {

        // Place stations
        // Position control points
        // Generate track points
        // Add lil train to move around between stations
        // Add track mesh?
        // Get lil train to point in the right direction around track

        GenerateTrackPoints();

        foreach (Vector3 p in trackPoints)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = p;
            g.transform.localScale = Vector3.one * spacing * 0.5f;
        }
    }

    // Adapted from Sebastian Lague's 2D curve editor code
    private void GenerateTrackPoints()
    {
        if (stations.Length < 2)
        {
            return; // not enough to form a loop!
        }

        List<Vector3> evenlySpacedPoints = new List<Vector3>();
        evenlySpacedPoints.Add(stations[0].transform.position);
        Vector3 previousPoint = stations[0].transform.position;
        float distanceSinceLastEvenPoint = 0;

        for (int i = 0; i < stations.Length; i++)
            {
                int nextIdx = (i + 1 + stations.Length) % stations.Length; // handle loop

                Debug.Log("Current: " + i.ToString() + " Next: " + nextIdx.ToString());

                Vector3[] p = new Vector3[] { stations[i].transform.position, 
                                              stations[i].postControl.transform.position,
                                              stations[nextIdx].preControl.transform.position,
                                              stations[nextIdx].transform.position,};

                float controlNetLength = Vector3.Distance(p[0], p[1]) + Vector3.Distance(p[1], p[2]) + Vector3.Distance(p[2], p[3]);
                float estimatedCurveLength = Vector3.Distance(p[0], p[3]) + (controlNetLength * 0.5f);
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
                        distanceSinceLastEvenPoint = overshootDist;
                        previousPoint = newEvenlySpacedPoint;
                    }
                    previousPoint = pointOnCurve;
                }
            }

            trackPoints = evenlySpacedPoints.ToArray();
        }

}




