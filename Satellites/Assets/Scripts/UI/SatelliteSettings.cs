using TMPro;
using UnityEngine;

public class SatelliteSettings : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nameField;
    
    [SerializeField] private TMP_InputField _massField;
    [SerializeField] private TMP_InputField _velocityField;

    [Space] 
    [SerializeField] private SatelliteCursor _cursor;
    
    [Space] 
    [SerializeField] private float _defaultMass;
    [SerializeField] private float _defaultVelocity;
    
    [Space] 
    [SerializeField] private Planetoid _satellitePrefab;

    private float _mass;
    private float _velocity;
    
    void Start()
    {
        _mass = _defaultMass;
        _velocity = _defaultVelocity;
        
        _massField.SetTextWithoutNotify(_mass+"");
        _velocityField.SetTextWithoutNotify(_velocity+"");
    }

    public void IncMass()
    {
        if (_mass > UIManager.MaxMass)
            return;
        
        _mass *= UIManager.ChangeRate;
        _massField.SetTextWithoutNotify(_mass+"");
    }

    public void DecMass()
    {
        if (_mass < 1)
            return;
        
        _mass /= UIManager.ChangeRate;
        _massField.SetTextWithoutNotify(_mass+"");
    }

    public void SetMass(string text)
    {
        float value = UIManager.ConvString(text, _mass);
        value = Mathf.Clamp(value, 1, UIManager.MaxMass);
            
        _mass = value;
        
        _massField.SetTextWithoutNotify(value+"");
    }

    //------------------------------------------------------
    
    public void IncVelocity()
    {
        if (_velocity > UIManager.MaxVelocity)
            return;
        
        _velocity *= UIManager.ChangeRate;
        _velocityField.SetTextWithoutNotify(_velocity+"");
    }

    public void DecVelocity()
    {
        _velocity /= UIManager.ChangeRate;
        _velocityField.SetTextWithoutNotify(_velocity+"");
    }

    public void SetVelocity(string text)
    {
        float value = UIManager.ConvString(text, _velocity);
        value = Mathf.Clamp(value, 0, UIManager.MaxVelocity);

        _velocity = value;
        
        _velocityField.SetTextWithoutNotify(value+"");
        
    }

    public void SetPosition()
    {
        _cursor.Activate();
    }

    public void CalculateOrbitVelocity()
    {
        if (!_cursor.gameObject.activeSelf)
            return;

        Planetoid planetoid = ViewManager.TargetPlanetoid;
        
        _velocity = Mathf.Sqrt(
            ((float)PhysicsManager.GravitationalConstant * planetoid.mass) /
            Vector3.Distance(planetoid.transform.position, _cursor.WorldPosition));
        
        _velocityField.SetTextWithoutNotify(_velocity+"");
    }

    public void CreateSatellite()
    {
        string title = _nameField.text;

        if (title.Length == 0)
            title = "Satellite";
        
        Planetoid planetoid = Instantiate(_satellitePrefab);
        planetoid.SetupRuntimePlanetoid(
            title, 
            _mass, 
            _cursor.WorldPosition + ViewManager.ViewOffset.ToVector3, 
            _cursor.Direction.normalized * _velocity + ViewManager.TargetPlanetoid.velocity.ToVector3);
        
        PhysicsManager.PlanetoidWasAdded.Invoke(planetoid);
        _cursor.Deactivate();
    }
}