using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconBehavior : MonoBehaviour
{
    public static bool DisableIcons;
    public GameObject[] DisableCameras;

    private LensFlare icon;

    void Start()
    {
        icon = GetComponent<LensFlare>();
    }

    void Update()
    {
        if (DisableIcons)
        {
            icon.brightness = 0;
            return;
        }

        foreach (GameObject cam in DisableCameras)
        {
            if (cam.activeSelf)
            {
                icon.brightness = 0;
                return;
            }
        }

        icon.brightness = 1;
    }
}