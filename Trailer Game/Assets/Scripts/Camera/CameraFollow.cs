using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private float smoothSpeed = 0.7f;
    private Vector3 offset;
    private Vector3 smoothInputVelocity = Vector3.zero;

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref smoothInputVelocity, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
