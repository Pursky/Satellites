using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunflareEffect : MonoBehaviour
{
    public float EffectDistance = 50;
    public float MaxBrightness = 5;

    private LensFlare lensFlare;

    void Start()
    {
        lensFlare = GetComponent<LensFlare>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);

        if (distance < EffectDistance)
        {
            lensFlare.brightness = ((MaxBrightness - 1) / Mathf.Pow(EffectDistance, 2)) * Mathf.Pow(distance - EffectDistance, 2) + 1;
        }
        else lensFlare.brightness = 1;
    }
}