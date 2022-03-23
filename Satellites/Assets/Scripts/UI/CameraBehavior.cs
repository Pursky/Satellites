using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform TargetMesh;
    public Vector3 StartRotation;
    public float MaxDistance;

    private float distance;
    private float rotationSpeed = 1;
    private float scrollSpeed;

    void Start()
    {
        distance = MaxDistance;
        transform.eulerAngles = StartRotation;
    }

    void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        scrollSpeed = distance/10f;

        if (scroll > 0) distance -= scrollSpeed;
        if (scroll < 0) distance += scrollSpeed;

        float maxDistance = MaxDistance;
        if (TargetMesh.localScale.x * 1.2f > MaxDistance) maxDistance = TargetMesh.localScale.x * 3;

        distance = Mathf.Clamp(distance, TargetMesh.localScale.x * 1.2f, maxDistance);

        if (Input.GetMouseButton(1))
        {
            transform.Rotate(new Vector3(-y, x) * rotationSpeed);

            float xRotation = transform.eulerAngles.x;
            if (xRotation > 180) xRotation -= 360;

            transform.eulerAngles = new Vector3(Mathf.Clamp(xRotation, -80, 80), transform.eulerAngles.y, 0);
        }

        transform.localPosition = Vector3.zero - transform.forward * distance;
    }
}