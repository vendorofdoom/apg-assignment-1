using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for procedurally generating a loop made of Bezier curve segments
// Adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

public class BezierLoop
{
    public List<BezierAnchor> anchors;



    public float spacing = 0.5f;
    public float resolution = 1f;
    public Vector3[] sampledPoints;
    public Vector3[] sampledDirs;
    public float[] segmentDists;

    public BezierLoop(int numAnchors)
    {
        PositionAnchors(numAnchors);
        PositionAllControlPoints();
        SampleCurves();
    }

    public int NumSegments
    {
        get
        {
            return anchors.Count;
        }
    }

    private void PositionAnchors(int numAnchors)
    {
        anchors = new List<BezierAnchor>();

        Vector3 origin = Vector3.zero;
        Vector3 dir = Vector3.forward;

        for (int i = 0; i < numAnchors; i++)
        {
            BezierAnchor a = new BezierAnchor();
            a.anchorPos = (origin + dir * Random.Range(10f, 20f)) + (Vector3.up * Random.Range(0f, 10f));
            anchors.Add(a);
            dir = Quaternion.Euler(0, 360f / numAnchors, 0) * dir;

        }
    }


    private void PositionControlPoints(int anchorIdx)
    {

        Vector3 currStation = anchors[anchorIdx].anchorPos;
        Vector3 dir = Vector3.zero;

        Vector3 prevOffset = anchors[LoopIdx(anchorIdx - 1)].anchorPos - currStation;
        Vector3 postOffset = anchors[LoopIdx(anchorIdx + 1)].anchorPos - currStation;

        dir = prevOffset - postOffset;
        dir.Normalize();

        anchors[anchorIdx].prevControlPointPos = currStation + dir * prevOffset.magnitude * 0.5f;
        anchors[anchorIdx].postControlPointPos = currStation + dir * -postOffset.magnitude * 0.5f;

    }

    private void PositionAllControlPoints()
    {
        for (int i = 0; i < anchors.Count; i++)
        {
            PositionControlPoints(i);
        }

    }

    public int LoopIdx(int i)
    {
        return (i + anchors.Count) % anchors.Count;
    }

    private void SampleCurves()
    {
        segmentDists = new float[anchors.Count];

        if (anchors.Count < 2)
        {
            return; // not enough to form a loop!
        }

        List<Vector3> evenlySpacedPoints = new List<Vector3>();
        evenlySpacedPoints.Add(anchors[0].anchorPos);
        Vector3 previousPoint = anchors[0].anchorPos;
        float distanceSinceLastEvenPoint = 0;

        List<Vector3> evenlySpacedForwards = new List<Vector3>();
        evenlySpacedForwards.Add((anchors[0].postControlPointPos - anchors[0].anchorPos).normalized); // vector looking at the next control point

        for (int i = 0; i < anchors.Count; i++)
        {
            int nextIdx = LoopIdx(i + 1);

            Vector3[] p = new Vector3[] { anchors[i].anchorPos,
                                              anchors[i].postControlPointPos,
                                              anchors[nextIdx].prevControlPointPos,
                                              anchors[nextIdx].anchorPos};

            float controlNetLength = Vector3.Distance(p[0], p[1]) + Vector3.Distance(p[1], p[2]) + Vector3.Distance(p[2], p[3]);
            float estimatedCurveLength = Vector3.Distance(p[0], p[3]) + (controlNetLength * 0.5f);
            segmentDists[i] = estimatedCurveLength;
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

        sampledPoints = evenlySpacedPoints.ToArray();
        sampledDirs = evenlySpacedForwards.ToArray();

    }
}
