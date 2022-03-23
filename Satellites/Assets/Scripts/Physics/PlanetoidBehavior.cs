using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetoidBehavior : MonoBehaviour
{
    public Transform OrbitRing;

    public float Diameter;
    public float Distance;
    public float RotationDuration;
    public float OrbitDuration;
    public float Mass;
    public float TrailWidth;

    private float ODiameter;
    private float ODistance;
    private float OOrbitDuration;
    private float ORotationDuration;
    private float OMass;

    private float angle;
    private RotationBehavior rotation;

    private void Awake()
    {
        rotation = transform.GetChild(0).GetComponent<RotationBehavior>();
        RotationDuration = rotation.RotationDuration;
        Diameter = rotation.transform.localScale.x;

        angle = Random.Range(0, 360f);

        ODiameter = Diameter;
        ODistance = Distance;
        ORotationDuration = RotationDuration;
        OOrbitDuration = OrbitDuration;
        OMass = Mass;
    }

    private void FixedUpdate()
    {
        if (OrbitDuration == 0) return;

        angle += Time.fixedDeltaTime * 360f / OrbitDuration * PhysicsBehavior.TimeScale;
        if (angle > 360) angle = 0;

        transform.localPosition = new Vector3(Sin(angle), 0, Cos(angle)) * Distance;
    }

    public void UpdateValues(float diameter, float distance, float rotationDuration, float orbitDuration, float mass)
    {
        Diameter = diameter;
        Distance = distance;
        RotationDuration = rotationDuration;
        OrbitDuration = orbitDuration;
        Mass = mass;

        if (OrbitRing != null) OrbitRing.localScale = new Vector3(distance, distance, distance) / 5;
        rotation.gameObject.transform.localScale = new Vector3(diameter, diameter, diameter);
        rotation.RotationDuration = RotationDuration;
    }

    public void UpdateValues()
    {
        UpdateValues(Diameter, Distance, RotationDuration, OrbitDuration, Mass);
    }

    public void ResetValues()
    {
        UpdateValues(ODiameter, ODistance, ORotationDuration, OOrbitDuration, OMass);
    }

    public float GetVelocity()
    {
        if (OrbitDuration == 0) return 0;

        return (2 * Distance * Mathf.PI) / OrbitDuration;
    }

    public Vector3 GetVelocityVector()
    {
        if (OrbitDuration == 0) return Vector3.zero;

        Vector3 vector = (Vector3.Cross(Vector3.up, transform.localPosition).normalized) * GetVelocity();
        if (transform.parent != null) vector += transform.parent.GetComponent<PlanetoidBehavior>().GetVelocityVector();
        return vector;
    }

    public float GetAngularMomentum()
    {
        return GetVelocity() * Mass * transform.localPosition.magnitude;
    }

    public float GetOriginalValue(int index)
    {
        switch (index)
        {
            case 0:
                return ODiameter;
            case 1:
                return ODistance;
            case 2:
                return ORotationDuration;
            case 3:
                return OOrbitDuration;
            case 4:
                return OMass;
        }
        return 0;
    }

    public float GetValue(int index)
    {
        switch (index)
        {
            case 0:
                return Diameter;
            case 1:
                return Distance;
            case 2:
                return RotationDuration;
            case 3:
                return OrbitDuration;
            case 4:
                return Mass;
        }
        return 0;
    }

    public void SetValue(int index, float value)
    {
        switch (index)
        {
            case 0:
                Diameter = value; 
                break;
            case 1:
                Distance = value;
                break;
            case 2:
                RotationDuration = value;
                break;
            case 3:
                OrbitDuration = value;
                break;
            case 4:
                Mass = value;
                break;
        }

        UpdateValues();
    }

    private float Sin(float x) => Mathf.Sin((x / 180f) * Mathf.PI);

    private float Cos(float x) => Mathf.Cos((x / 180f) * Mathf.PI);
}
