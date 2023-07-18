using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Tooltip _targetTooltip;
    [SerializeField] private string _text;
    
    [Space]
    [SerializeField] private bool _onlyIfInteractable;
    
    private RectTransform _transform;
    private Selectable _selectable;
    
    public Tooltip TargetTooltip {set => _targetTooltip = value;}

    public string Text {set => _text = value;}

    private void Awake()
    {
        _transform = (RectTransform)transform;
        _selectable = GetComponent <Selectable>();
        
        ViewManager.OnSelectPlanetoidFinish.AddListener((planetoid) => _targetTooltip.Deactivate());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_onlyIfInteractable || (_onlyIfInteractable && _selectable && _selectable.interactable))
            _targetTooltip.Activate(_text, _transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _targetTooltip.Deactivate();
    }
}