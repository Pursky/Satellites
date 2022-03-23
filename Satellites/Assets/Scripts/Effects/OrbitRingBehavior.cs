using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitRingBehavior : MonoBehaviour
{
    public static bool DisableOrbits;
    public GameObject[] DisableCameras;
    private GameObject upside;
    private GameObject underside;

    private void Start()
    {
        upside = transform.GetChild(0).gameObject;
        underside = transform.GetChild(1).gameObject;
    }

    void Update()
    {
        if (DisableOrbits)
        {
            upside.SetActive(false);
            underside.SetActive(false);
            return;
        }

        foreach(GameObject cam in DisableCameras)
        {
            if (cam.activeSelf)
            {
                upside.SetActive(false);
                underside.SetActive(false);

                return;
            }
        }

        upside.SetActive(true);
        underside.SetActive(true);
    }
}