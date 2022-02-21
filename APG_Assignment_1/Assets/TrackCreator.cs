using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

public class TrackCreator : MonoBehaviour
{
    [HideInInspector]
    public Track track;

    public void CreateTrack()
    {
        track = new Track(transform.position);
    }

}
