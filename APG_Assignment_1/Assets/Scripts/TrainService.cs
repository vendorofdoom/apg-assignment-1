using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainService : MonoBehaviour
{
    private ServiceType service;

    public int minCarriages;
    public int maxCarriages;

    public AudioSource chooChoo;

    public ServiceType Service
    {
        get
        {
            return service;
        }
        set
        {
            service = value;
            Configure();
        }
    }

    private void Start()
    {
        Configure();
    }

    public enum ServiceType
    {
        Local,
        National,
        Sleeper
    }

    private void Configure()
    {
        switch (service)
        {
            case ServiceType.Local:
                minCarriages = 3;
                maxCarriages = 5;
                chooChoo.pitch = Random.Range(1f, 1.2f);
                break;

            case ServiceType.National:
                minCarriages = 6;
                maxCarriages = 5;
                chooChoo.pitch = Random.Range(0.7f, 1f);
                break;

            case ServiceType.Sleeper:
                minCarriages = 11;
                maxCarriages = 20;
                chooChoo.pitch = Random.Range(0.5f, 0.7f);
                break;

            default:
                throw new System.NotImplementedException();

        }
    }

}
