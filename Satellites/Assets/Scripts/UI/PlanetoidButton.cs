using UnityEngine;
using UnityEngine.UI;

public class PlanetoidButton : MonoBehaviour
{
    private Planetoid _planetoid;
    private Button _button;

    private Image _image;
    private Image _subImage;

    private float _fadeDistance;
    private float _defaultOpacity;
    
    private void Awake()
    {
        _image = GetComponent <Image>();
        _subImage = transform.GetChild(0).GetComponent <Image>();

        _defaultOpacity = _image.color.a;
        
        ViewManager.OnSelectPlanetoidStart.AddListener(
        planetoid =>
        {
            _button.interactable = false;
            SetOpacity(1);
        });
        
        ViewManager.OnSelectPlanetoidFinish.AddListener(
        planetoid =>
        {
            if (planetoid != _planetoid)
                _button.interactable = true;
        });
        
        ViewManager.OnZoomedIn.AddListener(SetOpacity);
        
        _button = GetComponent <Button>();
        _button.onClick.AddListener(
        () =>
        {
            _button.interactable = false;
            ViewManager.TargetPlanetoid = _planetoid;
        });
    }

    private void Update()
    {
        Vector3 position = MainCamera.Camera.WorldToScreenPoint(_planetoid.transform.position);
        ToggleVisible(position.z > 0);

        position.z = 0;
        
        transform.position = position;
    }

    public void Initialize(Planetoid planetoid)
    {
        _planetoid = planetoid;
        GetComponent <Image>().sprite = _planetoid.Icon;

        if (!TryGetComponent(out TooltipTrigger trigger))
            return;

        trigger.TargetTooltip = UIManager.PlanetoidTooltip;
        trigger.Text = planetoid.Name;
    }

    private void ToggleVisible(bool visible)
    {
        _image.enabled = visible;
        _subImage.color = visible ? Color.white : Color.clear;
    }

    private void SetOpacity(float opacity)
    {
        _image.color = new Color(1, 1, 1, opacity * _defaultOpacity);
    }
}