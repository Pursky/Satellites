using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Planetoid : MonoBehaviour, IComparable <Planetoid>
{
    [SerializeField] private string _name;
    
    [Space]
    [Tooltip("in 100000 km")]public Vector3D position;
    [Tooltip("In km/s")]public Vector3D velocity;

    [Space] 
    [Tooltip("In kg²⁴")]public float mass;
    [SerializeField, Tooltip("In km/100000 s")]private float _initialSpeed;
    [SerializeField, Tooltip("In degrees")]private float _eccentricity;
    
    [Space]
    [SerializeField, Tooltip("In degrees")]private float _tilt;
    [SerializeField] private Planetoid _rotationAround;
    [SerializeField, Tooltip("in 100000 km")]private float _orbitRadius;

    [Space] 
    [SerializeField] private Sprite _icon;

    [HideInInspector] public int index;
    
    private Transform _transform;
    private Transform _button;

    private int _rotationParentCount;
    private bool _wasAddedInRuntime;

    public string Name => _name;
    public Sprite Icon => _icon;

    public bool WasAddedInRuntime => _wasAddedInRuntime;
    
    private void Awake()
    {
        _transform = transform;
        
        PhysicsManager.AddPlanetoid(this);
        Planetoid target = _rotationAround;

        while (target != null)
        {
            _rotationParentCount++;
            target = target._rotationAround;
        }
    }

    public void SetupRuntimePlanetoid(string pName, float pMass, Vector3 pPosition, Vector3 pVelocity)
    {
        _wasAddedInRuntime = true;
        _name = pName;
        
        mass = pMass;

        position = new Vector3D(pPosition);
        velocity = new Vector3D(pVelocity);

        index = PhysicsManager.GetIndex(this);

        _rotationParentCount = int.MaxValue;
        SpawnButton();
    }

    public void SetupPosition()
    {
        _transform.position = _rotationAround ? _rotationAround.transform.position : Vector3.zero;
        _transform.eulerAngles = Vector3.zero;
        
        _transform.Rotate(Vector3.up, Random.Range(0, 360));
        _transform.position += _transform.forward * _orbitRadius;
        
        _transform.Rotate(Vector3.up, 90);
        _transform.Rotate(_transform.right, _eccentricity);

        if (_rotationAround)
            velocity = _rotationAround.velocity;
        
        velocity += new Vector3D(_transform.forward * _initialSpeed);

        _transform.localEulerAngles = Vector3.right * _tilt;
        
        Vector3 transPosition = _transform.position;
        position = new Vector3D(transPosition);
        
        SpawnButton();
    }

    private void SpawnButton()
    {
        UIManager.SpawnButton(this);
    }

    public void AddForce(Vector3D force)
    {
        if (mass <= 0)
            return;
        
        velocity += force / mass * Time.deltaTime * PhysicsManager.TimeScale;
    }
    
    private void OnDestroy()
    {
        PhysicsManager.RemovePlanetoid(this);
    }

    void Update()
    {
        position += velocity * Time.deltaTime * PhysicsManager.TimeScale;
    }

    private void LateUpdate()
    {
        transform.position = (position - ViewManager.ViewOffset).ToVector3;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 50);
    }

    public int CompareTo(Planetoid other) => _rotationParentCount > other._rotationParentCount ? 1 : _rotationParentCount < other._rotationParentCount ? -1 : 0;
}