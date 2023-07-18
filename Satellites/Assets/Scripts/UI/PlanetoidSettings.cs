using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanetoidSettings : MonoBehaviour
{
    [SerializeField] private TMP_Text _planetoidName;
    [SerializeField] private Image _planetoidIcon;

    [Space]
    [SerializeField] private TMP_InputField _massField;
    [SerializeField] private TMP_InputField _velocityField;
    [SerializeField] private TMP_InputField _diameterField;

    [Space] 
    [SerializeField] private HoldButton _diameterIncButton;
    [SerializeField] private HoldButton _diameterDecButton;

    [Space] 
    [SerializeField] private TMP_Dropdown _planetoidDropdown;

    private void Awake()
    {
        ViewManager.OnSelectPlanetoidStart.AddListener(
        planetoid =>
        {
            _planetoidIcon.sprite = planetoid.Icon;
            _planetoidName.text = planetoid.Name;

            _massField.SetTextWithoutNotify(planetoid.mass+"");
            _diameterField.SetTextWithoutNotify(planetoid.transform.localScale.x * 100000f+"");
            
            _planetoidDropdown.SetValueWithoutNotify(planetoid.index);
            
            _diameterField.interactable = !planetoid.WasAddedInRuntime;
            _diameterIncButton.interactable = !planetoid.WasAddedInRuntime;
            _diameterDecButton.interactable = !planetoid.WasAddedInRuntime;

            if (planetoid.WasAddedInRuntime)
                _diameterField.text = "-";
        });
        
        PhysicsManager.PlanetoidWasAdded.AddListener(planetoid => UpdatePlanetoidList());
    }

    private void Start()
    {
        UpdatePlanetoidList();
    }

    void Update()
    {
        if (!_velocityField.isFocused)
            _velocityField.SetTextWithoutNotify(ViewManager.TargetPlanetoid.velocity.Magnitude.ToString("0.000"));
    }

    public void IncMass()
    {
        if (ViewManager.TargetPlanetoid.mass > UIManager.MaxMass)
            return;
        
        ViewManager.TargetPlanetoid.mass *= UIManager.ChangeRate;
        
        _massField.SetTextWithoutNotify(ViewManager.TargetPlanetoid.mass+"");
    }

    public void DecMass()
    {
        if (ViewManager.TargetPlanetoid.mass < 1)
            return;
        
        ViewManager.TargetPlanetoid.mass /= UIManager.ChangeRate;
        _massField.SetTextWithoutNotify(ViewManager.TargetPlanetoid.mass+"");
    }

    public void SetMass(string text)
    {
        float value = UIManager.ConvString(text, ViewManager.TargetPlanetoid.mass);
        value = Mathf.Clamp(value, 1, UIManager.MaxMass);
            
        ViewManager.TargetPlanetoid.mass = value;
        
        _massField.SetTextWithoutNotify(value+"");
    }

    //------------------------------------------------------
    
    public void IncVelocity()
    {
        if (ViewManager.TargetPlanetoid.velocity.Magnitude > UIManager.MaxVelocity)
            return;
        
        ViewManager.TargetPlanetoid.velocity *= UIManager.ChangeRate;
    }

    public void DecVelocity()
    {
        ViewManager.TargetPlanetoid.velocity /= UIManager.ChangeRate;
    }

    public void SetVelocity(string text)
    {
        float speed = (float)ViewManager.TargetPlanetoid.velocity.Magnitude;
        
        float value = UIManager.ConvString(text, speed);
        value = Mathf.Clamp(value, 0, UIManager.MaxVelocity);
        
        Vector3D velocity = ViewManager.TargetPlanetoid.velocity;
        ViewManager.TargetPlanetoid.velocity = velocity.Normalized * value;
    }
    
    //------------------------------------------------------

    public void IncDiameter()
    {
        if (ViewManager.TargetPlanetoid.transform.localScale.x > UIManager.MaxDiameter)
            return;
        
        Transform planetoid = ViewManager.TargetPlanetoid.transform;
        
        planetoid.localScale *= UIManager.ChangeRate;
        _diameterField.SetTextWithoutNotify(planetoid.localScale.x * 100000f+"");
    }

    public void DecDiameter()
    {
        Transform planetoid = ViewManager.TargetPlanetoid.transform;
        
        planetoid.localScale /= UIManager.ChangeRate;
        _diameterField.SetTextWithoutNotify(planetoid.localScale.x * 100000f+"");
    }

    public void SetDiameter(string text)
    {
        float value = UIManager.ConvString(text, ViewManager.TargetPlanetoid.transform.localScale.x);
        value = Mathf.Clamp(value, 0, UIManager.MaxDiameter * 100000f);
        
        ViewManager.TargetPlanetoid.transform.localScale = Vector3.one * value / 100000f;
        _diameterField.SetTextWithoutNotify(value+"");
    }
    
    //------------------------------------------------------

    public void SelectPlanetoid(int index)
    {
        ViewManager.TargetPlanetoid = PhysicsManager.Planetoids[index];
    }

    private void UpdatePlanetoidList()
    {
        Planetoid[] planetoids = PhysicsManager.Planetoids;
        List <TMP_Dropdown.OptionData> data = new();

        foreach (Planetoid planetoid in planetoids)
            data.Add(new TMP_Dropdown.OptionData(planetoid.Name, planetoid.Icon));
        
        _planetoidDropdown.options = data;
    }
}