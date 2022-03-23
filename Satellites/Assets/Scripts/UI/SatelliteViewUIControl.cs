using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SatelliteViewUIControl : MonoBehaviour
{
    public PhysicsBehavior PhysicsBehavior;
    public Dropdown SatelliteSelect;
    public Button DestroyButton;
    public InputField[] PositionFields;
    public InputField VelocityField;
    public InputField MassField;
    public Text SatelliteName;

    void Start()
    {
        
    }

    void Update()
    {
        if (PhysicsBehavior.Satellites.Count == 0) return;

        for (int i = 0; i < PhysicsBehavior.Satellites.Count; i++)
        {
            if (i == SatelliteSelect.value) PhysicsBehavior.Satellites[i].Icon.color = Color.yellow;
            else PhysicsBehavior.Satellites[i].Icon.color = Color.gray;
        }

        int index = SatelliteSelect.value;

        SatelliteName.text = PhysicsBehavior.Satellites[index].name;

        VelocityField.text = PhysicsBehavior.Satellites[index].Velocity.magnitude * 1000+ "";
        MassField.text = PhysicsBehavior.Satellites[index].Mass * 1000 + "";

        PositionFields[0].text = PhysicsBehavior.Satellites[index].transform.position.x + "";
        PositionFields[1].text = PhysicsBehavior.Satellites[index].transform.position.y + "";
        PositionFields[2].text = PhysicsBehavior.Satellites[index].transform.position.z + "";
    }

    public void UpdateOptions()
    {
        SatelliteSelect.ClearOptions();

        if (PhysicsBehavior.Satellites.Count == 0)
        {
            SatelliteSelect.interactable = false;
            DestroyButton.interactable = false;

            SatelliteName.text = "";

            VelocityField.text = "";
            MassField.text = "";

            PositionFields[0].text = "";
            PositionFields[1].text = "";
            PositionFields[2].text = "";

            return;
        }

        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

        foreach(SatelliteBehavior satellite in PhysicsBehavior.Satellites)
        {
            options.Add(new Dropdown.OptionData(satellite.name));
        }

        SatelliteSelect.AddOptions(options);
        SatelliteSelect.value = options.Count - 1;

        SatelliteSelect.interactable = true;
        DestroyButton.interactable = true;
    }

    public void DestroySatellite()
    {
        PhysicsBehavior.DestroySatellite(PhysicsBehavior.Satellites[SatelliteSelect.value]);
    }
}