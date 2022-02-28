using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainService : MonoBehaviour
{
    private ServiceType service;

    public int minCarriages;
    public int maxCarriages;

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
                break;

            case ServiceType.National:
                minCarriages = 6;
                maxCarriages = 5;
                break;

            case ServiceType.Sleeper:
                minCarriages = 11;
                maxCarriages = 20;
                break;

            default:
                throw new System.NotImplementedException();

        }
    }

}
