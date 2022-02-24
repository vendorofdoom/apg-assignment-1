using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for procedurally generating a loop made of Bezier curve segments
// Adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

public class BezierLoop
{

    public Vector3[] anchors;
    public Vector3[] controlPoints;

    public float spacing = 0.5f;
    public float resolution = 1f;
    public Vector3[] sampledPoints;
    public Vector3[] sampledDirs;
    public float[] segmentDists;


    public Vector3 AnchorPoint(int i)
    {
        return anchors[LoopIdx(i)];
    }

    public Vector3 PrevControlPoint(int i)
    {
        return controlPoints[2 * LoopIdx(i)];
    }

    public Vector3 PostControlPoint(int i)
    {
        return controlPoints[2 * LoopIdx(i) + 1];
    }

    public BezierLoop(int numAnchors)
    {
        anchors = new Vector3[numAnchors];
        controlPoints = new Vector3[numAnchors * 2];

        PositionAnchors(numAnchors);
        PositionAllControlPoints();
        SampleCurves();
    }

    private void PositionAnchors(int numAnchors)
    {
        Vector3 origin = Vector3.zero;
        Vector3 dir = Vector3.forward;

        for (int i = 0; i < numAnchors; i++)
        {
            Vector3 a = (origin + dir * Random.Range(10f, 20f)) + (Vector3.up * Random.Range(0f, 10f));
            anchors[i] = a;
            dir = Quaternion.Euler(0, 360f / numAnchors, 0) * dir;
        }
    }


    private void PositionControlPoints(int anchorIdx)
    {

        Vector3 currStation = AnchorPoint(anchorIdx);
        Vector3 dir = Vector3.zero;

        Vector3 prevOffset = AnchorPoint(anchorIdx - 1) - currStation;
        Vector3 postOffset = AnchorPoint(anchorIdx + 1) - currStation;

        dir = prevOffset - postOffset;
        dir.Normalize();

        controlPoints[anchorIdx*2] = currStation + dir * prevOffset.magnitude * 0.5f;
        controlPoints[anchorIdx*2+1] = currStation + dir * -postOffset.magnitude * 0.5f;

    }

    private void PositionAllControlPoints()
    {
        for (int i = 0; i < anchors.Length; i++)
        {
            PositionControlPoints(i);
        }

    }

    public int LoopIdx(int i)
    {
        return (i + anchors.Length) % anchors.Length;
    }

    private void SampleCurves()
    {
        segmentDists = new float[anchors.Length];

        if (anchors.Length < 2)
        {
            return; // not enough to form a loop!
        }

        List<Vector3> evenlySpacedPoints = new List<Vector3>();
        evenlySpacedPoints.Add(anchors[0]);
        Vector3 previousPoint = anchors[0];
        float distanceSinceLastEvenPoint = 0;

        List<Vector3> evenlySpacedForwards = new List<Vector3>();
        evenlySpacedForwards.Add((controlPoints[1] - anchors[0]).normalized); // vector looking from the first anchor point to the next control point

        for (int i = 0; i < anchors.Length; i++)
        {

            Vector3[] p = new Vector3[] { AnchorPoint(i),
                                          PostControlPoint(i),
                                          PrevControlPoint(i+1),
                                          AnchorPoint(i+1)};

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



    public Vector3[] PosAndForwardForTime(float t)
    {
        float totalT = 1f * anchors.Length;


        return new Vector3[] { Vector3.one, Vector3.one };
    }


}
