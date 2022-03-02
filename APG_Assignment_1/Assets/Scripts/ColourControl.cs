using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColourControl : MonoBehaviour
{
    public Gradient stationColours;
    public Gradient cloudColours;
    public Gradient trainColours;

    public void PaintTrain(Transform train)
    {
        float seed = Random.Range(0f, 1f);

        // Try to select colours far enough apart in the gradient so they look distinct
        Color wheels = trainColours.Evaluate(seed);
        Color train1 = trainColours.Evaluate((seed + 0.33f) % 1f);
        Color train2 = trainColours.Evaluate((seed + 0.66f) % 1f);

        foreach (MeshRenderer mr in train.GetComponentsInChildren<MeshRenderer>())
        {
            foreach(Material m in mr.materials)
            {

                if (m.name.ToLower().Contains("wheel"))
                {
                    m.color = wheels;
                }
                else if (!m.name.ToLower().Contains("train_1"))
                {
                    m.color = train1;
                }
                else if (!m.name.ToLower().Contains("train_2"))
                {
                    m.color = train2;
                }
            }
        }
    }

    public void PaintStations(List<Station> stations)
    {
        foreach (Station s in stations)
        {
            foreach (MeshRenderer mr in s.transform.Find("StationOffset").transform.GetComponentsInChildren<MeshRenderer>())
            {
                foreach (Material m in mr.materials)
                {
                    if (m.name.ToLower().Contains("cloud"))
                    {
                        m.color = cloudColours.Evaluate(Random.Range(0f, 1f));
                    }
                    else if (!m.name.ToLower().Contains("glass"))
                    {
                        m.color = stationColours.Evaluate(Random.Range(0f, 1f));
                    }
                }
            }
        }
    }

    public void PaintTrackAndDetails(Transform trackParent)
    {
        float seed = Random.Range(0f, 1f);
        Color train1 = trainColours.Evaluate((seed) % 1f);
        Color train2 = trainColours.Evaluate((seed + 0.5f) % 1f);

        foreach (MeshRenderer mr in trackParent.GetComponentsInChildren<MeshRenderer>())
        {
            foreach (Material m in mr.materials)
            {
                if (m.name.ToLower().Contains("cloud"))
                {
                    m.color = cloudColours.Evaluate(Random.Range(0f, 1f));
                }
                else if (!m.name.ToLower().Contains("train_1"))
                {
                    m.color = train1;
                }
                else if (!m.name.ToLower().Contains("train_2"))
                {
                    m.color = train2;
                }
            }
        }
    }
}
