using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-10)]
public class ViewManager : MonoBehaviour
{
    private static ViewManager s_instance;

    [SerializeField] private Planetoid _initialPlanetoid;
    [SerializeField] private float _viewAlignSpeed;
    
    [Space]
    [SerializeField] private Material _trailMaterial;
    [SerializeField] private float _trailWidthMult;
    [SerializeField] private float _trailInterval;
    [SerializeField] private float _trailMaxPositionCount;

    [Space] 
    [SerializeField] private bool _showOrbits;
    [SerializeField] private bool _showLocalOrbits;
    
    [Space]
    [SerializeField] private int _fadeOutScaleMult;
    
    private Vector3D _viewOffset;

    private UnityEvent <Planetoid> _onSelectPlanetoidStart;
    private UnityEvent <Planetoid> _onSelectPlanetoidFinish;
    private UnityEvent <float> _onZoomedIn;
    private UnityEvent <bool> _onOrbitModeChanged;
    private UnityEvent <bool> _onShowOrbitChanged;
    
    private Planetoid _targetPlanetoid;
    private bool _alignIsRunning;
    
    public static Vector3D ViewOffset => s_instance._viewOffset;
    public static UnityEvent <Planetoid> OnSelectPlanetoidStart => s_instance._onSelectPlanetoidStart;
    public static UnityEvent <Planetoid> OnSelectPlanetoidFinish => s_instance._onSelectPlanetoidFinish;
    public static UnityEvent <float> OnZoomedIn => s_instance._onZoomedIn;
    public static UnityEvent <bool> OnOrbitModeChanged => s_instance._onOrbitModeChanged;
    public static UnityEvent <bool> OnShowOrbitChanged => s_instance._onShowOrbitChanged;

    public static Material TrailMaterial => s_instance._trailMaterial;
    public static float TrailWidthMult => s_instance._trailWidthMult;
    public static float TrailInterval => s_instance._trailInterval;
    public static float TrailMaxPositionCount => s_instance._trailMaxPositionCount;
    public static bool AlignIsRunning => s_instance._alignIsRunning;

    public static Planetoid TargetPlanetoid
    {
        get => s_instance._targetPlanetoid;
        set
        {
            if (s_instance._targetPlanetoid == value)
                return;
        
            s_instance.StartCoroutine(s_instance.AlignViewToNewTarget(value));
        }
    }

    public static bool ShowLocalOrbits
    {
        get => s_instance._showLocalOrbits;
        set
        {
            s_instance._showLocalOrbits = value;
            s_instance._onOrbitModeChanged.Invoke(value);
        }
    }
    
    public static bool ShowOrbits
    {
        get => s_instance._showOrbits;
        set
        {
            s_instance._showOrbits = value;
            s_instance._onShowOrbitChanged.Invoke(value);
        }
    }

    private void Awake()
    {
        if (!s_instance)
            s_instance = this;
        else
            Destroy(this);
        
        _onSelectPlanetoidStart = new UnityEvent <Planetoid>();
        _onSelectPlanetoidFinish = new UnityEvent <Planetoid>();
        
        _onZoomedIn = new UnityEvent <float>();
        
        _onOrbitModeChanged = new UnityEvent <bool>();
        _onShowOrbitChanged = new UnityEvent <bool>();
    }

    private void Start()
    {
        TargetPlanetoid = _initialPlanetoid;
    }

    private void Update()
    {
        float fadeDistance = _targetPlanetoid.transform.localScale.x * s_instance._fadeOutScaleMult;
        
        if (MainCamera.CameraDistance < fadeDistance)
            _onZoomedIn.Invoke(2 * MainCamera.CameraDistance / fadeDistance - 1);
    }

    private void LateUpdate()
    {
        if (_alignIsRunning)
            return;

        _viewOffset = _targetPlanetoid.position;
    }

    private IEnumerator AlignViewToNewTarget(Planetoid newTarget)
    {
        Planetoid oldTarget = s_instance._targetPlanetoid;
        
        s_instance._onSelectPlanetoidStart.Invoke(newTarget);
        s_instance._targetPlanetoid = newTarget;
        
        _alignIsRunning = true;
        
        float timer = 0;

        if (oldTarget)
        {
            while (timer < 1)
            {
                timer += Time.deltaTime * _viewAlignSpeed;
                _viewOffset = Vector3D.Lerp(oldTarget.position, _targetPlanetoid.position, -0.5f * Mathf.Cos(timer * Mathf.PI) + 0.5f);
                
                yield return null;
            }
        }

        _alignIsRunning = false; 
        _viewOffset = _targetPlanetoid.position;
        
        _onSelectPlanetoidFinish.Invoke(_targetPlanetoid);
    }

    private void OnValidate()
    {
        _onOrbitModeChanged?.Invoke(_showLocalOrbits);
        _onShowOrbitChanged?.Invoke(_showOrbits);
    }
}