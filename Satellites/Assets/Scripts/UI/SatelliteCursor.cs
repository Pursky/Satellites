using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SatelliteCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private InputActionProperty _mousePositionAction;
    [SerializeField] private InputActionProperty _placeAction;

    [Space] 
    [SerializeField] private HoldButton _rotateXButton;
    [SerializeField] private HoldButton _rotateYButton;
    [SerializeField] private HoldButton _translateButton;
    
    [Space]
    [SerializeField] private float _lineLengthMultiplier;
    [SerializeField] private float _lineWidthMultiplier;

    private Vector3 _worldPosition;
    private Vector3 _direction;
    
    private Plane _plane;
    private bool _isRayCasting;
    private LineRenderer _line;

    private Image _image;

    public Vector3 WorldPosition => _worldPosition;

    public Vector3 Direction => _direction;
    private Vector2 MousePosition => _mousePositionAction.action.ReadValue <Vector2>();
    
    private void Awake()
    {
        _direction = Vector3.forward;
        
        _image = GetComponent <Image>();
        _line = GetComponent <LineRenderer>();
        
        ToggleButtons(false);
        gameObject.SetActive(false);

        _line.positionCount = 2;
        
        _placeAction.action.performed += context =>
        {
            if (_isRayCasting)
                ToggleButtons(true);
            
            _isRayCasting = false;
        };

        _rotateXButton.OnPointerUpEvent.AddListener(() => DeactivateButtonsIfUnHovered(_rotateXButton));
        _rotateYButton.OnPointerUpEvent.AddListener(() => DeactivateButtonsIfUnHovered(_rotateYButton));
        _translateButton.OnPointerUpEvent.AddListener(() => DeactivateButtonsIfUnHovered(_translateButton));
    }

    void Update()
    {
        float distance = Vector3.Distance(_worldPosition, MainCamera.Camera.transform.position) * _lineWidthMultiplier;
        _line.endWidth = distance;
        
        _line.SetPosition(0, _worldPosition);
        _line.SetPosition(1, _worldPosition + _direction * (distance * _lineLengthMultiplier));
        
        Vector2 mousePosition = MousePosition;
        
        if (!_isRayCasting)
        {
            Vector3 position = MainCamera.Camera.WorldToScreenPoint(_worldPosition);
            ToggleVisible(position.z > 0);

            position.z = 0;
        
            transform.position = position;
        }
        else
        {
            _plane = new Plane(Vector3.up, ViewManager.TargetPlanetoid.transform.position);

            Ray ray = MainCamera.Camera.ScreenPointToRay(mousePosition);

            if (!_plane.Raycast(ray, out float enter))
                return;

            _worldPosition = ray.GetPoint(enter);
            
            transform.position = mousePosition;
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        _isRayCasting = true;
        ToggleButtons(false);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
        _isRayCasting = false;
        ToggleButtons(false);
        
        _direction = Vector3.forward;
    }

    public void Rotate(bool onX)
    {
        _plane = onX ? new Plane(Vector3.up, _worldPosition) :
                        new Plane(Vector3.Cross(Vector3.up, _direction), _worldPosition);

        Ray ray = MainCamera.Camera.ScreenPointToRay(MousePosition);

        if (!_plane.Raycast(ray, out float enter))
            return;

        Vector3 point = ray.GetPoint(enter);
        _direction = (point - _worldPosition).normalized;
    }

    public void Translate()
    {
        Vector3 normal = MainCamera.Camera.transform.forward;
        normal.y = 0;
        
        _plane = new Plane(normal, _worldPosition);
        
        Ray ray = MainCamera.Camera.ScreenPointToRay(MousePosition);

        if (!_plane.Raycast(ray, out float enter))
            return;
        
        Vector3 point = ray.GetPoint(enter);
        _worldPosition.y = point.y;
    }

    private void ToggleVisible(bool enable)
    {
        _image.enabled = enable;
    }

    private void ToggleButtons(bool enable)
    {
        _rotateXButton.gameObject.SetActive(enable);
        _rotateYButton.gameObject.SetActive(enable);
        _translateButton.gameObject.SetActive(enable);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isRayCasting)
            ToggleButtons(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_rotateXButton.PointerIsDown && !_rotateYButton.PointerIsDown && !_translateButton.PointerIsDown)
            ToggleButtons(false);
    }

    private void DeactivateButtonsIfUnHovered(HoldButton button)
    {
        if (!button.PointerIsDown)
            ToggleButtons(false);
    }
}