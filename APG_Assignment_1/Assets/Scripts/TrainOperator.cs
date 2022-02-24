using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainOperator : MonoBehaviour
{
    public TrainTrack track;
    public float speed;

    public GameObject locomotivePrefab;
    public GameObject[] carriagePrefabs;

    [Range(1, 8)]
    public int numCarriages = 1;
    public float carriageSpacing = 1.2f;

    [SerializeField]
    private Transform[] carriages;
    private float[] t;

    private void Start()
    {
        SetupTrain();
    }


    private void SetupTrain()
    {
        carriages = new Transform[numCarriages];
        t = new float[numCarriages];

        float startT = 0f;

        for (int i = numCarriages - 1; i >= 0; i--)
        {
            // set up the prefab for the carriage
            GameObject carriageObj;
            if (i == 0)
            {
                carriageObj = GameObject.Instantiate(locomotivePrefab, transform);
            }
            else
            {
                carriageObj = GameObject.Instantiate(carriagePrefabs[Random.Range(0, carriagePrefabs.Length)], transform);
            }

            carriages[i] = carriageObj.GetComponent<Transform>();

            t[i] = startT;

            // place the game object
            carriages[i].position = track.bezierLoop.AnchorPoint(0);

            startT += carriageSpacing;
        }


    }

    private void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            DestroyTrain();
            SetupTrain();
        }

        for (int i = 0; i < numCarriages; i++)
        {
            t[i] += Time.deltaTime * speed;

            Vector3 position;
            Vector3 forward;
            track.bezierLoop.PosAndForwardForDistance(t[i], out position, out forward);

            carriages[i].position = position;
            carriages[i].forward = forward;
        }
    }

    private void DestroyTrain()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
