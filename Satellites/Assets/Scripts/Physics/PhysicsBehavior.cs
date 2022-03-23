using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBehavior : MonoBehaviour
{
    public static float GravConst = 0.66743f;

    public static float TimeScale = 1;
    public PlanetoidBehavior[] Planets;
    public List<SatelliteBehavior> Satellites;
    public SatelliteViewUIControl SatelliteViewUI;

    void Start()
    {

    }

    void Update()
    {
        if (Satellites.Count == 0) return;

        foreach(SatelliteBehavior satellite in Satellites) 
        {
            foreach (PlanetoidBehavior planet in Planets)
            {
                Vector3 difVector = planet.transform.position - satellite.transform.position;
                float force = GravConst * ((planet.Mass) / Mathf.Pow(difVector.magnitude, 2));
                satellite.Velocity += difVector.normalized * force * Time.deltaTime * TimeScale;
            }

            foreach (SatelliteBehavior satellite2 in Satellites)
            {
                if (satellite != satellite2)
                {
                    Vector3 difVector = satellite2.transform.position - satellite.transform.position;
                    float force = GravConst * ((satellite2.Mass) / Mathf.Pow(difVector.magnitude, 2));
                    satellite.Velocity += difVector.normalized * force * Time.deltaTime * TimeScale;
                }
            }
        }
    }

    public void AddSatellite(SatelliteBehavior satellite)
    {
        Satellites.Add(satellite);
        SatelliteViewUI.UpdateOptions();
    }

    public void DestroySatellite(SatelliteBehavior satellite)
    {
        Satellites.Remove(satellite);
        Destroy(satellite.gameObject);
        SatelliteViewUI.UpdateOptions();
    }
}

// 1 time unit      = 100000 seconds
// 1 distance unit  = 100000 km
// 1 mass unit      = 1kg²⁴
// 1 speed unit     = 1000 m/s = 1 km/s