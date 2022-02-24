using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brackeys Smooth Camera Follow: https://www.youtube.com/MFQhpwc6cKE

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private Transform originalPos;

    [Range(0f, 1f)]
    public float smoothSpeed = 0.125f;

    private void Start()
    {
        originalPos = transform;
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = target.rotation;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, smoothSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = originalPos.position;
            transform.rotation = originalPos.rotation;
        }
    }

}
