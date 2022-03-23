using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SatelliteUIControl : MonoBehaviour
{
    public GameObject SatellitePrefab;
    public GameObject PositionDummyPrefab;
    public InputField[] PositionFields;
    public Slider[] RotationSliders;
    public InputField[] RotationFields;
    public PhysicsBehavior PhysicsBehavior;
    public InputField NameField;
    public InputField VelocityField;
    public InputField MassField;
    public float YModSpeed;
    public PlanetoidBehavior Planet;

    private Camera activeCamera;
    private Transform positionDummy;
    private LineRenderer velocityLine;
    private Vector3 relativeDummyPosition;
    private bool positionIsSet;
    private bool raisingY;
    private bool loweringY;

    void Start()
    {
        PositionFields[0].text = "0";
        PositionFields[1].text = "0";
        PositionFields[2].text = "0";

        RotationFields[0].text = "0";
        RotationFields[1].text = "0";

        VelocityField.text = "0";
        MassField.text = "1";
    }

    void Update()
    {
        RotationFields[0].text = RotationSliders[0].value+"";
        RotationFields[1].text = RotationSliders[1].value+"";

        if (positionDummy == null)
        {
            PositionFields[0].text = "0";
            PositionFields[1].text = "0";
            PositionFields[2].text = "0";
            return;
        }

        if (raisingY) ChangeY(true);
        if (loweringY) ChangeY(false);

        PositionFields[0].text = positionDummy.position.x+"";
        PositionFields[1].text = positionDummy.position.y+"";
        PositionFields[2].text = positionDummy.position.z+"";

        positionDummy.eulerAngles = new Vector3(-RotationSliders[1].value, RotationSliders[0].value, 0);

        if (positionIsSet)
        {
            Vector3 targetPos = Planet.transform.position + relativeDummyPosition;
            positionDummy.transform.position = new Vector3(targetPos.x, positionDummy.position.y, targetPos.z);
            return;
        }

        Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit hit, 100000);
        positionDummy.transform.position = new Vector3(hit.point.x, positionDummy.position.y, hit.point.z);

        if (Input.GetMouseButtonDown(0))
        {
            positionIsSet = true;
            relativeDummyPosition = positionDummy.position - Planet.transform.position;
        }
    }

    public void ActivatePositionDummy()
    {
        if (positionDummy == null)
        {
            positionDummy = Instantiate(PositionDummyPrefab, Vector3.zero, Quaternion.identity).transform;
            positionDummy.name = "PositionDummy";
            velocityLine = positionDummy.GetComponent<LineRenderer>();

            velocityLine.endWidth = Planet.TrailWidth;
            velocityLine.SetPosition(1, Vector3.forward * Planet.TrailWidth * 10);

            ResetRotationSliders();
        }
        positionIsSet = false;
        positionDummy.position = new Vector3(positionDummy.position.x, 0, positionDummy.position.z);
    }

    public void SetCamera(Camera cam)
    {
        activeCamera = cam;
        Planet = activeCamera.transform.parent.GetComponent<PlanetoidBehavior>();
        if (positionDummy != null) Destroy(positionDummy.gameObject);

        RotationFields[0].text = "0";
        RotationFields[1].text = "0";

        ResetRotationSliders();
    }

    public void ChangeY(bool up)
    {
        if (positionDummy == null) return;

        float modifier = Planet.Diameter * YModSpeed * Time.deltaTime * (Vector3.Distance(Planet.transform.position, positionDummy.position) / Planet.Diameter);
        if (!up) modifier *= -1;

        positionDummy.position = new Vector3(positionDummy.position.x, positionDummy.position.y + modifier, positionDummy.position.z);
    }

    public void ModifyY(int type)
    {
        switch (type)
        {
            case 0: 
                raisingY = true;
                break;
            case 1:
                raisingY = false;
                break;
            case 2:
                loweringY = true;
                break;
            case 3:
                loweringY = false;
                break;
        }
    }

    public void ResetRotationSliders()
    {
        RotationSliders[0].value = 0;
        RotationSliders[1].value = 0;
    }

    public void ModifyVelocity()
    {
        if (!float.TryParse(VelocityField.text, out float value)) VelocityField.text = "0";
    }

    public void ModifyMass()
    {
        if (!float.TryParse(MassField.text, out float value)) MassField.text = "1";
        if (value < 0) MassField.text = "1";
    }

    public void SpawnSatellite()
    {
        if (positionDummy == null) return;

        Update();
        ModifyMass();
        ModifyVelocity();

        SatelliteBehavior satellite = Instantiate(SatellitePrefab, positionDummy.position, Quaternion.identity).GetComponent<SatelliteBehavior>();
        satellite.Start();
        satellite.Trail.SatelliteUI = this;
        satellite.PhysicsBehavior = PhysicsBehavior;
        satellite.name = NameField.text;
        if (NameField.text.Equals("")) satellite.name = "Satellite";
        satellite.Trail.Center = Planet.transform;
        satellite.Velocity = positionDummy.forward * float.Parse(VelocityField.text) / 1000f + Planet.GetVelocityVector();
        satellite.Mass = float.Parse(MassField.text) / 1000f;
        PhysicsBehavior.AddSatellite(satellite);

        Destroy(positionDummy.gameObject);
    }

    public void CalculateOrbitVelocity()
    {
        if (positionDummy == null) return;
        Update();
        float velocity = Mathf.Sqrt((PhysicsBehavior.GravConst*Planet.Mass)/Vector3.Distance(Planet.transform.position, positionDummy.position));
        VelocityField.text = velocity * 1000 + "";
    }
}