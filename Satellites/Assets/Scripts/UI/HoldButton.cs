using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoldButton : Selectable
{
    [SerializeField] private UnityEvent _onHold;
    [SerializeField] private UnityEvent _onPointerDown;
    [SerializeField] private UnityEvent _onPointerUp;

    private bool _pointerIsDown;
    
    public UnityEvent OnHoldEvent => _onHold;
    public UnityEvent OnPointerDownEvent => _onPointerDown;
    public UnityEvent OnPointerUpEvent => _onPointerUp;

    public bool PointerIsDown => _pointerIsDown;

    void Update()
    {
        if (!_pointerIsDown)
            return;
        
        _onHold.Invoke();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!interactable)
            return;
        
        _pointerIsDown = true;
        _onPointerDown.Invoke();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!interactable)
            return;
        
        _pointerIsDown = false;
        _onPointerUp.Invoke();
    }
}