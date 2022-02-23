using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/d9k97JemYbM

public static class Bezier
{

    public static Vector3 EvaluateQuadratic(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p0 = Vector3.Lerp(a, b, t);
        Vector3 p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    public static Vector3 EvaluateCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p0 = EvaluateQuadratic(a, b, c, t);
        Vector3 p1 = EvaluateQuadratic(b, c, d, t);
        return Vector3.Lerp(p0, p1, t);

    }

    // https://chowdera.com/2021/12/202112220035289689.html
    public static Vector3 TangentCubic(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {

        float u = 1 - t;
        float uu = u * u;
        float tu = t * u;
        float tt = t * t;

        Vector3 P = a * 3 * uu * (-1.0f);
        P += b * 3 * (uu - 2 * tu);
        P += c * 3 * (2 * tu - tt);
        P += d * 3 * tt;

        return P.normalized;
    }
}
