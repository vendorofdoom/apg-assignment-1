using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeOfDay : MonoBehaviour
{
    public Camera mainCam;

    public Camera trainCam; // TODO: get this filled in when train cam is setup

    public MeshRenderer trainHeadlights;
    public Material trainHeadlightsOn;
    public Material trainHeadlightsOff;

    public Gradient sky;
    public Light sun; // TODO: rotate position based on time of day?

    private float timeOfDay;
    public Slider timeOfDaySlider;

    public void SliderChange()
    {
        timeOfDay = timeOfDaySlider.value;
    }

    void Start()
    {
        timeOfDay = Random.Range(0f, 1f);
        timeOfDaySlider.value = timeOfDay;
        SetColours();
    }

    void Update()
    {
        SetColours();
    }

    public void SetColours()
    {
        mainCam.backgroundColor = sky.Evaluate(timeOfDay);
        sun.intensity = timeOfDay;
        if (trainHeadlights != null)
        {
            if (timeOfDay <= 0.3)
            {
                trainHeadlights.material = trainHeadlightsOn;
            }
            else
            {
                trainHeadlights.material = trainHeadlightsOff;
            }
        }
        if (trainCam != null)
        {
            trainCam.backgroundColor = sky.Evaluate(timeOfDay);
        }

    }
}