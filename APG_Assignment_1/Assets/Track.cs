using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

[System.Serializable]
public class Track
{
    [SerializeField, HideInInspector]
    List<Vector3> points;

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
            return (points.Count - 4) / 3 + 1;
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
        return new Vector3[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[i * 3 + 3] };
    }

    public void MovePoint(int i, Vector3 newPos)
    {
        points[i] = newPos;
    }

    //public Transform[] controlPoints;
    //public Transform train;

    //[Range(0f, 1f)]
    //public float t;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    train.position = controlPoints[0].position;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    train.position = CubicLerp(controlPoints[0].position, controlPoints[1].position, controlPoints[2].position, controlPoints[3].position, t);
    //}

    //public Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    //{
    //    Vector3 p0 = Vector3.Lerp(a, b, t);
    //    Vector3 p1 = Vector3.Lerp(b, c, t);
    //    return Vector3.Lerp(p0, p1, t);
    //}

    //public Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    //{
    //    Vector3 p0 = QuadraticLerp(a, b, c, t);
    //    Vector3 p1 = QuadraticLerp(b, c, d, t);
    //    return Vector3.Lerp(p0, p1, t);
    //}
}
