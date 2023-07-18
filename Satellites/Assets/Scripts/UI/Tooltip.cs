using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    [SerializeField] private float _heightOffset;
    
    private RectTransform _transform;
    
    private TMP_Text _text;
    private RectTransform _target;

    private Vector3 _offset;

    private void Awake()
    {
        _transform = (RectTransform)transform;
        _text = GetComponentInChildren <TMP_Text>();
        
        gameObject.SetActive(false);
    }

    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        _transform.position = _target.position + _offset * UIManager.PixelMultiplier;
    }

    public void Activate(string text, RectTransform target)
    {
        gameObject.SetActive(true);
        
        _text.text = text;
        _target = target;

        _offset = Vector3.up * (target.rect.height * 0.5f + _transform.rect.height * 0.5f + _heightOffset);
        
        UpdatePosition();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}