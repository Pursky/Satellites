using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    private Graphic _graphic;

    [SerializeField] private float _fadeTime = 1;
    [SerializeField] private bool _startFadedIn;

    private Color _originalColor;
    
    private void Awake()
    {
        _graphic = GetComponent <Graphic>();
        
        if (_startFadedIn)
            SetFadedIn();
        else
            SetFadedOut();

        _originalColor = _graphic.color;
    }

    public void FadeOut(UnityAction completeAction = null)
    {
        StopAllCoroutines();
        StartCoroutine(Fade(false, completeAction));
    }

    public void FadeIn(UnityAction completeAction = null)
    {
        _graphic.enabled = true;
        
        StopAllCoroutines();
        StartCoroutine(Fade(true, completeAction));
    }

    public void SetFadedIn()
    {
        SetOpacity(1);
        _graphic.enabled = true;
    }

    public void SetFadedOut()
    {
        SetOpacity(0);
        _graphic.enabled = false;
    }

    private IEnumerator Fade(bool fadeIn, UnityAction completeAction)
    {
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime / _fadeTime;
            SetOpacity(fadeIn ? timer : -timer + 1);
            
            yield return null;
        }
        
        completeAction?.Invoke();
        
        if (fadeIn)
            SetFadedIn();
        else 
            SetFadedOut();
    }

    private void SetOpacity(float value)
    {
        _graphic.color = new Color(_originalColor.r, _originalColor.g, _originalColor.b, value);
    }
}