using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Parts of this script are adapted for 3D from Sebastian Lague 2D Curve Editor Tutorial Series: https://youtu.be/RF04Fi9OCPc

public class TrainTrack : MonoBehaviour
{
    [Header("Bezier Loop")]
    public BezierLoop bezierLoop;
    public Style style;

    private int maxAnchorPoints;
    private int minDist;
    private int maxDist;
    private int maxHeight;

    [Header("Stations")]
    public Transform stationParent;
    public GameObject stationPrefab;
    public StationNameGenerator stationNameGen;
    public List<Station> stations;
    public List<int> stationPointIdx; // station distance around the track counting from the first evenly spaced point
 
    [Header("Track")]
    public Transform trackParent;
    public GameObject trackPrefab;

    [Header("Scenery")]
    public GameObject[] sceneryPrefabs;
    public bool drawScenery = false;

    public ColourControl colourControl;
    public TimeOfDay tod;

    private void Awake()
    {
        TrackStyle = Style.Gentle;
    }

    public void GentleTrack()
    {
        TrackStyle = TrainTrack.Style.Gentle;
    }

    public void ModerateTrack()
    {
        TrackStyle = TrainTrack.Style.Moderate;
    }

    public void WackyTrack()
    {
        TrackStyle = TrainTrack.Style.Wacky;
    }

    public void RegenerateTrack()
    {
        ClearTracksAndStations();
        Configure();
        bezierLoop = new BezierLoop(maxAnchorPoints, minDist, maxDist, maxHeight);
        DrawTrack();
        colourControl.PaintStations(stations);
        colourControl.PaintTrackAndDetails(trackParent);
    }

    public Style TrackStyle
    {
        get
        {
            return style;
        }
        set
        {
            style = value;
            //Configure();
            RegenerateTrack();
        }
    }

    public enum Style
    {
        Gentle,
        Moderate,
        Wacky
    }

    private void Configure()
    {
        switch (style)
        {
            case Style.Gentle:
                maxAnchorPoints = 4;
                minDist = 5;
                maxDist = 20;
                maxHeight = 0;
                break;

            case Style.Moderate:
                maxAnchorPoints = 8;
                minDist = 10;
                maxDist = 20;
                maxHeight = 5;
                break;

            case Style.Wacky:
                maxAnchorPoints = 15;
                minDist = 10;
                maxDist = 20;
                maxHeight = 10;
                break;

            default:
                throw new System.NotImplementedException();


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

        tod.lamps = new List<MeshRenderer>();

        stations = new List<Station>();
        stationPointIdx = new List<int>();

        int gapLength = 0;

        for (int i = 0; i < bezierLoop.sampledPoints.Length; i++)

        {
            GameObject g = GameObject.Instantiate(trackPrefab, trackParent);
            g.transform.position = bezierLoop.sampledPoints[i];
            g.transform.localScale *= (bezierLoop.spacing * 2f);
            g.transform.forward = bezierLoop.sampledDirs[i];

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

        // TODO: implement less hacky way of making sure we don't have overlapping objects where the
        // start and end of the loop meet
        for (int i = 0; i < bezierLoop.sampledPoints.Length - 5; i++) 
        {
            if (drawScenery)
            {
                if (gapLength > 0)
                {
                    gapLength--;
                }
                else
                {
                    if (!TooCloseToStations(bezierLoop.sampledPoints[i], 5f))
                    {
                        GameObject t = GameObject.Instantiate(sceneryPrefabs[Random.Range(0, sceneryPrefabs.Length)], trackParent);
                        t.transform.position = bezierLoop.sampledPoints[i];
                        t.transform.localScale *= (bezierLoop.spacing * 2f);
                        t.transform.forward = bezierLoop.sampledDirs[i];
                        gapLength = Random.Range(10, 50); // make this a parameter?

                        if (t.name.ToLower().Contains("lamp"))
                        {
                            foreach (MeshRenderer mr in t.GetComponentsInChildren<MeshRenderer>())
                            {
                                if (mr.name.ToLower().Contains("lamp"))
                                {
                                    tod.lamps.Add(mr);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private bool TooCloseToStations(Vector3 itemToPlace, float minDist)
    {
        bool tooCloseToAnotherStation = false;

        // I couldn't get collider checks working here? Maybe something to do with physics calcs if stuff being placed in the same frame?
        foreach (Station s in stations)
        {
            if (Vector3.Distance(s.transform.position, itemToPlace) < minDist)
            {
                tooCloseToAnotherStation = true;
                break;
            }
        }

        return tooCloseToAnotherStation;
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




