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
    public BezierLoop bezierLoop;

    [Header("Stations")]
    public int numStations;
    public Transform stationParent;
    public GameObject stationPrefab;

    [Header("Track")]
    public Transform trackParent;
    public GameObject trackPrefab;

    private void Awake()
    {
        bezierLoop = new BezierLoop(8);
        DrawTrack();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ClearTracksAndStations();
            bezierLoop = new BezierLoop(8);
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

    private void DrawTrack()
    {
        for (int i = 0; i < bezierLoop.sampledPoints.Length; i++)
        {
            GameObject g = GameObject.Instantiate(trackPrefab, trackParent);
            g.transform.position = bezierLoop.sampledPoints[i];
            g.transform.localScale *= (bezierLoop.spacing * 2f);
            g.transform.forward = bezierLoop.sampledDirs[i];
        }
    }

}




