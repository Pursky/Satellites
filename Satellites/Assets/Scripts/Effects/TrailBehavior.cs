using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailBehavior : MonoBehaviour
{
    public Transform Center;
    public SatelliteUIControl SatelliteUI;

    private LineRenderer line;
    private Queue<Vector3> positions;
    private float timer;
    private float timerInterval = 0.03f;

    void Start()
    {
        timer = timerInterval;
        line = GetComponent<LineRenderer>();
        positions = new Queue<Vector3>();
        for (int i = 0; i < line.positionCount; i++) positions.Enqueue(transform.position - Center.position);
        line.endWidth = SatelliteUI.Planet.TrailWidth;
    }

    void Update()
    {
        if (OrbitRingBehavior.DisableOrbits)
        {
            line.enabled = false;
        }
        else
        {
            line.enabled = true;
        }

        timer -= Time.deltaTime;

        Transform temp = Center;
        Center = SatelliteUI.Planet.transform;
        if (temp != Center) ResetTrail();

        if (timer <= 0)
        {
            positions.Enqueue(transform.position - Center.position);
            timer = timerInterval;
        }

        Vector3[] positionArray = positions.ToArray();
        for (int i = 0; i < line.positionCount; i++) line.SetPosition(i, Center.position + positionArray[i]);
        if (positions.Count > line.positionCount) positions.Dequeue();

        line.SetPosition(line.positionCount - 1, transform.position);
    }

    public void ResetTrail()
    {
        positions.Clear();
        for (int i = 0; i < line.positionCount; i++) positions.Enqueue(transform.position - Center.position);
        line.endWidth = SatelliteUI.Planet.TrailWidth;
    }
}