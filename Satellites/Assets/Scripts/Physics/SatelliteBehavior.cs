using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatelliteBehavior : MonoBehaviour
{
    public PhysicsBehavior PhysicsBehavior;
    public TrailBehavior Trail;
    public Vector3 Velocity;
    public float Mass;
    public LensFlare Icon;

    private Rigidbody rigbod;

    public void Start()
    {
        Trail = GetComponent<TrailBehavior>();
        Icon = GetComponent<LensFlare>();
        rigbod = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rigbod.MovePosition(transform.position + Velocity * Time.fixedDeltaTime * PhysicsBehavior.TimeScale);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.name.Equals("RayLayer"))
        {
            PhysicsBehavior.DestroySatellite(this);
        }
    }
}
