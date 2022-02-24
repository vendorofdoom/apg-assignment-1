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

    [Header("Tunnels")]
    public GameObject tunnelPrefab;

    private void Awake()
    {
        bezierLoop = new BezierLoop(15, 10, 20, 10);
        DrawTrack();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ClearTracksAndStations();
            bezierLoop = new BezierLoop(15, 10, 20, 10);
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

        bool placeTunnelPiece = false;
        int tunnelLength = 0;

        for (int i = 0; i < bezierLoop.sampledPoints.Length; i++)
        {
            GameObject g = GameObject.Instantiate(trackPrefab, trackParent);
            g.transform.position = bezierLoop.sampledPoints[i];
            g.transform.localScale *= (bezierLoop.spacing * 2f);
            g.transform.forward = bezierLoop.sampledDirs[i];

            if (placeTunnelPiece)
            {
               
                if (tunnelLength >= 0)
                {
                    GameObject t = GameObject.Instantiate(tunnelPrefab, trackParent);
                    t.transform.position = bezierLoop.sampledPoints[i];
                    t.transform.localScale *= (bezierLoop.spacing * 2f);
                    t.transform.forward = bezierLoop.sampledDirs[i];
                    tunnelLength--;
                }
                else
                {
                    placeTunnelPiece = false;
                }

            }
            else
            {
                if (Random.Range(0f, 1f) >= 0.98f)
                {
                    placeTunnelPiece = true;
                    tunnelLength = Random.Range(5, 10); // make this a parameter?
                }
            }



        }
    }

}




