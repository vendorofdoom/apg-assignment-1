using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Brackeys Smooth Camera Follow: https://www.youtube.com/MFQhpwc6cKE

public class CameraController : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    private Vector3 originalPos;
    private Quaternion originalRot;

    [Range(0f, 1f)]
    public float smoothSpeed = 0.8f;

    private void Start()
    {
        originalPos = new Vector3(0, 20, -30);
        originalRot = Quaternion.Euler(new Vector3(35f, 0, 0));
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target.position);
        }
    }

    private void OnEnable()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
            transform.LookAt(target.position);
        }
    }

    private void OnDisable()
    {
        transform.position = originalPos;
        transform.rotation = originalRot;
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothSpeed * Time.deltaTime);
            transform.LookAt(target.position);
        }
    }

}
