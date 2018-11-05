using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlRotate : MonoBehaviour
{
    private const float Y_ANGLE_MIN = 0.0f;
    private const float Y_ANGLE_MAX = 50.0f;

    public Vector3 lookAt;
    public Transform camTransform;
    public float distance = 10.0f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    //private float sensitivityX = 4.0f;
    //private float sensitivityY = 1.0f;

    private void Start()
    {
        camTransform = transform;
        lookAt = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X");
            currentY += Input.GetAxis("Mouse Y");

            currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
        }
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        camTransform.position = lookAt + rotation * dir;
        camTransform.LookAt(lookAt);
    }
}
