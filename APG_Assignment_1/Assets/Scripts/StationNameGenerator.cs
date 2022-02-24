using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationNameGenerator : MonoBehaviour
{
    public List<string> adjectives;
    public List<string> prefixes;
    public List<string> suffixes;
    public List<string> places;

    public string GenerateName()
    {
        float picker = Random.Range(0f, 1f);

        if (picker <= 0.25f)
        {
            return adjectives[Random.Range(0, adjectives.Count)] + " " + prefixes[Random.Range(0, prefixes.Count)] + suffixes[Random.Range(0, suffixes.Count)];
        }
        else if (picker > 0.25f && picker <= 0.5f)
        {
            return prefixes[Random.Range(0, prefixes.Count)] + suffixes[Random.Range(0, suffixes.Count)] + " " + places[Random.Range(0, places.Count)];
        }
        else
        {
            return prefixes[Random.Range(0, prefixes.Count)] + suffixes[Random.Range(0, suffixes.Count)];
        }

    }
}
