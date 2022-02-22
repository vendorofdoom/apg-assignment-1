using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPlacer : MonoBehaviour
{
    public float spacing = 0.1f;
    public float resolution = 1f;

    private void Start()
    {
        Vector3[] points = FindObjectOfType<TrackCreator>().track.CalculateEvenlySpacedPoints(spacing, resolution);
        foreach (Vector3 p in points)
        {
            GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            g.transform.position = p;
            g.transform.localScale = Vector3.one * spacing * 0.5f;
        }
    }
}
