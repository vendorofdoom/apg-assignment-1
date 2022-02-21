using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

[System.Serializable]
public class Track
{
    [SerializeField, HideInInspector]
    List<Vector3> points;

    [SerializeField, HideInInspector]
    bool isClosed;

    public Track(Vector3 centre)
    {
        points = new List<Vector3>
        {
            centre + Vector3.left,
            centre + (Vector3.left + Vector3.up) * 0.5f,
            centre + (Vector3.right + Vector3.down) * 0.5f,
            centre + Vector3.right
        };
    }

    public Vector3 this[int i]
    {
        get
        {
            return points[i];
        }
    }

    public int NumPoints
    {
        get
        {
            return points.Count;
        }
    }

    public int NumSegments
    {
        get
        {
            return points.Count / 3;
        }
    }

    public void AddSegment(Vector3 anchorPos)
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add((points[points.Count - 1] + anchorPos) * 0.5f);
        points.Add(anchorPos);
    }

    public Vector3[] GetPointsInSegment(int i)
    {
        return new Vector3[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)] };
    }

    public void MovePoint(int i, Vector3 newPos)
    {
        Vector3 deltaMove = newPos - points[i];
        points[i] = newPos;

        if (i%3 == 0) // moving an anchor points so move control points too
        {
            if (i + 1 < points.Count || isClosed)
            {
                points[LoopIndex(i + 1)] += deltaMove;
            }
            if (i - 1 >= 0 || isClosed)
            {
                points[LoopIndex(i - 1)] += deltaMove;
            } 
        }
        else
        {
            bool nextPointIsAnchor = (i + 1) % 3 == 0;
            int correspondingControlIdx = (nextPointIsAnchor) ? i + 2 : i - 2;
            int anchorIdx = (nextPointIsAnchor) ? i + 1 : i - 1;

            if (correspondingControlIdx >= 0 && correspondingControlIdx < points.Count || isClosed)
            {

                float distance = (points[LoopIndex(anchorIdx)] - points[LoopIndex(correspondingControlIdx)]).magnitude;
                Vector3 direction = (points[LoopIndex(anchorIdx)] - newPos).normalized;
                points[LoopIndex(correspondingControlIdx)] = points[LoopIndex(anchorIdx)] + direction * distance;

            }

        }


    }

    public void ToggleClosed()
    {
        isClosed = !isClosed;

        if (isClosed)
        {
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add(points[0] * 2 - points[1]);
        }
        else
        {
            points.RemoveRange(points.Count - 2, 2);
        }

    }
    
    int LoopIndex(int i)
    {
        return (i + points.Count) % points.Count;
    }
}
