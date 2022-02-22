using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

public class TrackCreator : MonoBehaviour
{
    [HideInInspector]
    public Track track;

    public Color anchorCol = Color.red;
    public Color controlCol = Color.white;
    public Color segmentCol = Color.green;
    public Color selectedSegmentCol = Color.yellow;
    public float anchorDiameter = 0.1f;
    public float controlDiameter = 0.075f;
    public bool displayControlPoints = true;

    public void CreateTrack()
    {
        track = new Track(transform.position);
    }
     void Reset()
    {
        CreateTrack();
    }
}
