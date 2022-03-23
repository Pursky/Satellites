using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetUIControl : MonoBehaviour
{
    public GameObject[] Cameras;
    public Image Fade;
    public Dropdown CameraSelect;
    public Slider TimeScaleSlider;
    public Text TimeScaleSliderText2;
    public float FadeTime;
    public float MaxTimeScale;
    public float ModifyScale;
    public Text PlanetBoxName;
    public InputField[] PlanetBoxFields;
    public Text ControllsButtonText;
    public GameObject ControllsBox;
    
    private SatelliteUIControl satelliteUI;
    private bool fading;
    private float timer;

    private void Start()
    {
        Fade.color = Color.black;
        WritePlanetValues(0);
        satelliteUI = GetComponent<SatelliteUIControl>();
        satelliteUI.SetCamera(Cameras[0].GetComponent<Camera>());
    }

    void Update()
    {
        if (!fading)
        {
            if (Fade.color.a > 0) Fade.color -= Color.black * Time.deltaTime / FadeTime;
            return;
        }

        timer += Time.deltaTime;

        if (timer <= FadeTime) Fade.color += Color.black * Time.deltaTime / FadeTime;
        if (timer > FadeTime)
        {
            ChangeCamera();
            fading = false;
            timer = 0;
        }
    }

    public void StartCameraTransition()
    {
        if (Cameras[CameraSelect.value].activeSelf) return;
        fading = true;
    }

    public void ChangeCamera()
    {
        foreach (GameObject cam in Cameras) cam.SetActive(false);
        Cameras[CameraSelect.value].SetActive(true);
        satelliteUI.SetCamera(Cameras[CameraSelect.value].GetComponent<Camera>());
        WritePlanetValues(CameraSelect.value);
    }

    public void ChangeTimeScale()
    {
        PhysicsBehavior.TimeScale =  Mathf.Pow(TimeScaleSlider.value, Mathf.Log(MaxTimeScale, 2));
        
        TimeScaleSliderText2.text = $"1 : {Mathf.Round(PhysicsBehavior.TimeScale * 100000)}";
    }

    public void ResetTimeScale()
    {
        TimeScaleSlider.value = 1;
        ChangeTimeScale();
    }

    public void ToggleIcons()
    {
        IconBehavior.DisableIcons = !IconBehavior.DisableIcons;
    }

    public void ToggleOrbits()
    {
        OrbitRingBehavior.DisableOrbits = !OrbitRingBehavior.DisableOrbits;
    }

    public void WritePlanetValues(int index)
    {
        PlanetoidBehavior planet = GetPlanet(index);

        PlanetBoxName.text = planet.gameObject.name;

        PlanetBoxFields[0].text = planet.Diameter*100000+"";
        PlanetBoxFields[1].text = planet.Distance*100000+"";
        PlanetBoxFields[2].text = planet.RotationDuration * 100000f / 3600f+"";
        PlanetBoxFields[3].text = planet.OrbitDuration * 100000f / 86400f+"";
        PlanetBoxFields[4].text = planet.Mass+"";

        PlanetBoxFields[5].text = planet.GetVelocity() * 1000 + "";
        PlanetBoxFields[6].text = planet.GetAngularMomentum() + "";
    }

    public PlanetoidBehavior GetPlanet(int index)
    {
        return Cameras[index].transform.parent.GetComponent<PlanetoidBehavior>();
    }

    public void ChangePlanetValues()
    {
        PlanetoidBehavior planet = GetPlanet(CameraSelect.value);

        if (float.TryParse(PlanetBoxFields[0].text, out float diameter) && diameter >= 0) diameter /= 100000;
        else diameter = planet.Diameter;

        if (float.TryParse(PlanetBoxFields[1].text, out float distance) && distance >= 0) distance /= 100000;
        else distance = planet.Distance;

        if (float.TryParse(PlanetBoxFields[2].text, out float rotationDuration) && rotationDuration >= 0) rotationDuration *= 0.036f;
        else rotationDuration = planet.RotationDuration;

        if (float.TryParse(PlanetBoxFields[3].text, out float orbitDuration) && orbitDuration >= 0) orbitDuration *= 0.864f;
        else orbitDuration = planet.OrbitDuration;

        if (!float.TryParse(PlanetBoxFields[4].text, out float mass) || mass < 0) mass = planet.Mass;

        planet.UpdateValues(diameter, distance, rotationDuration, orbitDuration, mass);

        WritePlanetValues(CameraSelect.value);
    }

    public void ResetPlanetValues()
    {
        GetPlanet(CameraSelect.value).ResetValues();
        WritePlanetValues(CameraSelect.value);
    }

    public void IncreaseValue(int index)
    {
        PlanetoidBehavior planet = GetPlanet(CameraSelect.value);
        float originalValue = planet.GetOriginalValue(index);
        float value = planet.GetValue(index);

        float compare = 0;
        if (originalValue > 0) compare = value / originalValue;

        value += originalValue / ModifyScale * compare;
        planet.SetValue(index, value);

        WritePlanetValues(CameraSelect.value);
    }

    public void DecreaseValue(int index)
    {
        PlanetoidBehavior planet = GetPlanet(CameraSelect.value);
        float originalValue = planet.GetOriginalValue(index);
        float value = planet.GetValue(index);

        float compare = 0;
        if (originalValue > 0) compare = value / originalValue;

        value -= originalValue / ModifyScale * compare;
        if (value < 0) value = 0;
        planet.SetValue(index, value);

        WritePlanetValues(CameraSelect.value);
    }

    public void ToggleControlls()
    {
        ControllsBox.SetActive(!ControllsBox.activeSelf);
        if (ControllsBox.activeSelf) ControllsButtonText.text = "Hide Controlls";
        else ControllsButtonText.text = "Show Controlls";
    }

    public void Quit()
    {
        Application.Quit();
    }
}
