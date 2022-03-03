using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeOfDay : MonoBehaviour
{
    public Camera mainCam;

    public Camera trainCam; // TODO: get this filled in when train cam is setup

    public MeshRenderer trainHeadlights;
    public Material lightsOn;
    public Material lightsOff;

    public List<MeshRenderer> lamps;

    public Gradient sky;
    public Light sun; // TODO: rotate position based on time of day?

    private float timeOfDay;
    public Slider timeOfDaySlider;


    public ParticleSystemRenderer trainSmoke;
    public Material daySmoke;
    public Material nightSmoke;

    public void SliderChange()
    {
        timeOfDay = timeOfDaySlider.value;
        SetColours();
    }

    void Start()
    {
        timeOfDay = Random.Range(0f, 1f);
        timeOfDaySlider.value = timeOfDay;
        SetColours();
    }

    public void SetColours()
    {
        mainCam.backgroundColor = sky.Evaluate(timeOfDay);
        sun.intensity = timeOfDay;

        if (trainHeadlights != null && trainSmoke != null)
        {
            if (timeOfDay <= 0.3)
            {
                trainHeadlights.material = lightsOn;
                trainSmoke.material = nightSmoke;
            }
            else
            {
                trainHeadlights.material = lightsOff;
                trainSmoke.material = daySmoke;
            }
        }

        if (trainCam != null)
        {
            trainCam.backgroundColor = sky.Evaluate(timeOfDay);
        }

        if (lamps != null)
        {
            foreach (MeshRenderer lamp in lamps)
            {
                if (timeOfDay <= 0.3)
                {
                    lamp.material = lightsOn;
                }
                else
                {
                    lamp.material = lightsOff;
                }

            }
        }


    }
}
