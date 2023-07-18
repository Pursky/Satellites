using UnityEngine;
using UnityEngine.InputSystem;

public class MainCamera : MonoBehaviour
{
    private static MainCamera s_instance;
    
    [SerializeField] private InputActionProperty _dragAction;
    [SerializeField] private InputActionProperty _deltaMouseAction;
    [SerializeField] private InputActionProperty _zoomAction;

    [Space] 
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _maxZoomSpeed;
    [SerializeField] private float _zoomAcceleration;
    
    [Space]
    [SerializeField] private float _minDistanceScaleMult;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _distanceMultiplier;

    private Camera _camera;
    
    private Transform _transform;
    private float _cameraDistance;
    private Vector2 _zoomRange;
    private Vector2 _targetZoomRange;
    private bool _zoomIsBlocked;

    private float _currentZoomSpeed;

    public static float CameraDistance => s_instance._cameraDistance;
    public static Camera Camera => s_instance._camera;
    public static bool ZoomIsBlocked {set => s_instance._zoomIsBlocked = value;}

    private void Awake()
    {
        if (!s_instance)
            s_instance = this;
        else
            Destroy(this);

        _transform = transform;
        _camera = GetComponent <Camera>();
        
        _targetZoomRange.y = _maxDistance;
        _zoomRange = _targetZoomRange;
        
        _transform.eulerAngles = Vector3.right * 45;
        _cameraDistance = _maxDistance * 0.75f;
    }

    void Update()
    {
        if (_dragAction.action.IsPressed())
        {
            Vector2 deltaMouse = _deltaMouseAction.action.ReadValue <Vector2>() * _rotationSpeed;
            
            Vector3 rotation = _transform.eulerAngles;
            
            rotation.y += deltaMouse.x;
            rotation.x -= deltaMouse.y;
            rotation.z = 0;

            if (rotation.x is > 90 and < 180)
                rotation.x = 90;
            
            if (rotation.x is < 270 and > 180)
                rotation.x = 270;

            _transform.eulerAngles = rotation;
        }

        float zoomValue = _zoomIsBlocked ? 0 : _zoomAction.action.ReadValue <float>();
        
        _targetZoomRange.x = ViewManager.TargetPlanetoid.transform.localScale.x * _minDistanceScaleMult;

        _zoomRange = Vector2.Lerp(_zoomRange, _targetZoomRange, Time.deltaTime * 10);
        
        _currentZoomSpeed = Mathf.Lerp(_currentZoomSpeed, zoomValue * _maxZoomSpeed, _zoomAcceleration * Time.deltaTime);
        
        _cameraDistance -= _currentZoomSpeed * _cameraDistance * _distanceMultiplier;
        _cameraDistance = Mathf.Clamp(_cameraDistance, _zoomRange.x, _zoomRange.y);
        
        _transform.position = _transform.forward * -_cameraDistance;
    }
}