using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationBehavior : MonoBehaviour
{
    public float RotationDuration;

    void FixedUpdate()
    {
        if (RotationDuration == 0) return;
        transform.Rotate(new Vector3(0, Time.fixedDeltaTime * 360f / RotationDuration * PhysicsBehavior.TimeScale, 0));
    }
}