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
    public Transform stationParent;
    public GameObject stationPrefab;
    public StationNameGenerator stationNameGen;
    public List<Station> stations;
    public List<int> stationPointIdx; // station distance around the track counting from the first evenly spaced point
 
    [Header("Track")]
    public Transform trackParent;
    public GameObject trackPrefab;

    [Header("Tunnels")]
    public GameObject tunnelPrefab;
    public bool drawTunnels = false;

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
        stations = new List<Station>();
        stationPointIdx = new List<int>();

        bool placeTunnelPiece = false;
        int tunnelLength = 0;

        for (int i = 0; i < bezierLoop.sampledPoints.Length; i++)
        {
            GameObject g = GameObject.Instantiate(trackPrefab, trackParent);
            g.transform.position = bezierLoop.sampledPoints[i];
            g.transform.localScale *= (bezierLoop.spacing * 2f);
            g.transform.forward = bezierLoop.sampledDirs[i];

            if (drawTunnels)
            {
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

            
            if (Random.Range(0f, 1f) >= 0.9f 
                && (Vector3.Dot(g.transform.up.normalized, Vector3.up) >= 0.95f)) // only place a station if the spot is relatively "upright"
            {

                bool tooCloseToAnotherStation = false;
                float minDist = 20f; // TODO: parameterise?

                // I couldn't get collider checks working here? Maybe something to do with physics calcs if stuff being placed in the same frame?
                foreach (Station s in stations) 
                {
                    if (Vector3.Distance(s.transform.position, bezierLoop.sampledPoints[i]) < minDist)
                    {
                        tooCloseToAnotherStation = true;
                        break;
                    }
                }

                if (!tooCloseToAnotherStation)
                {
                    BuildAStation(bezierLoop.sampledPoints[i], bezierLoop.sampledDirs[i]);
                    stationPointIdx.Add(i);
                }
            }

        }
    }

    private void BuildAStation(Vector3 pos, Vector3 forward)
    {
        GameObject s = GameObject.Instantiate(stationPrefab, stationParent);
        s.transform.position = pos;
        s.transform.localScale *= (bezierLoop.spacing * 2f);
        s.transform.forward = forward;
        string stationName = stationNameGen.GenerateName();
        s.name = stationName + " Station";
        s.GetComponent<Station>().stationName = stationName;
        stations.Add(s.GetComponent<Station>());
    }

    public int StationAtDistance(float d)
    {
        int sampleIdx = bezierLoop.SampleIndex(d);
        int stationIdx = stationPointIdx.FindIndex(x => (x == sampleIdx));

        return stationIdx;
    }

}




