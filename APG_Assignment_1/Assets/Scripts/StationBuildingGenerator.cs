using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationBuildingGenerator : MonoBehaviour
{
    public GameObject[] RoofPrefabs;
    public GameObject[] BuildingPrefabs;
    public GameObject[] FacadePrefabs;
    public GameObject[] CloudPrefabs;

    public Transform parent;

    private void Awake()
    {
        GameObject.Instantiate(RoofPrefabs[Random.Range(0, RoofPrefabs.Length)], parent);
        GameObject.Instantiate(BuildingPrefabs[Random.Range(0, BuildingPrefabs.Length)], parent);
        GameObject.Instantiate(FacadePrefabs[Random.Range(0, FacadePrefabs.Length)], parent);
        GameObject.Instantiate(CloudPrefabs[Random.Range(0, CloudPrefabs.Length)], parent);
    }
}
