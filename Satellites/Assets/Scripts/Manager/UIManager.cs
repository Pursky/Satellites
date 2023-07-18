using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DefaultExecutionOrder(-20)]
public class UIManager : MonoBehaviour
{
    private static UIManager s_instance;

    [SerializeField] private Transform _buttonCanvas;
    [SerializeField] private PlanetoidButton _buttonPrefab;
    
    [Space] 
    [SerializeField] private Slider _timeSlider;
    [SerializeField] private TMP_Text _timeSliderText;
    
    [Space] 
    [SerializeField] private double _logarithmicTimeScale;
    
    [Space] 
    [SerializeField] private Tooltip _planetoidTooltip;
    
    [Space]
    [SerializeField] private float _maxMass;
    [SerializeField] private float _maxVelocity;
    [SerializeField] private float _maxDiameter;
    
    [Space] 
    [SerializeField] private float _valueChangeRate;
    
    [Space] 
    [SerializeField] private Fader _uiFader;

    private float _defaultTimeSliderValue;

    public static Tooltip PlanetoidTooltip => s_instance._planetoidTooltip;

    public static float PixelMultiplier => Screen.height / 1080f;

    public static float MaxMass => s_instance._maxMass;
    public static float MaxVelocity => s_instance._maxVelocity;
    public static float MaxDiameter => s_instance._maxDiameter;
    
    public static float ChangeRate => 1 + s_instance._valueChangeRate * Time.deltaTime;

    public static Fader UIFader => s_instance._uiFader;
    
    private void Awake()
    {
        if (!s_instance)
            s_instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        _timeSlider.minValue = (float)Math.Pow(PhysicsManager.TimeScaleMin, 1d / _logarithmicTimeScale);
        _timeSlider.maxValue = (float)Math.Pow(PhysicsManager.TimeScaleMax, 1d / _logarithmicTimeScale);

        _defaultTimeSliderValue = (float)Math.Pow(PhysicsManager.TimeScale, 1d / _logarithmicTimeScale);
        _timeSlider.value = _defaultTimeSliderValue;
        
        UpdateTimeSliderText();
        _uiFader.FadeOut();
    }

    public void SetTimeScale(float value)
    {
        PhysicsManager.TimeScale = Mathf.Pow(value, (float)_logarithmicTimeScale);
        UpdateTimeSliderText();
    }

    public void ResetTimeScale()
    {
        _timeSlider.value = _defaultTimeSliderValue;
    }

    public void ToggleShowOrbits(bool enable)
    {
        ViewManager.ShowOrbits = enable;
    }

    public void ToggleOrbitMode(bool enable)
    {
        ViewManager.ShowLocalOrbits = enable;
    }

    private void UpdateTimeSliderText()
    {
        _timeSliderText.text = $"1 : {Math.Round(PhysicsManager.TimeScale * 100000)}";
    }

    public void ResetAll()
    {
        _uiFader.FadeIn(() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void Quit()
    {
        _uiFader.FadeIn(Application.Quit);
    }

    public static void SpawnButton(Planetoid planetoid)
    {
        PlanetoidButton button = Instantiate(s_instance._buttonPrefab, s_instance._buttonCanvas);
        button.Initialize(planetoid);
        
        button.transform.SetSiblingIndex(0);
    }
    
    public static float ConvString(string text, float fallbackValue)
    {
        if (!float.TryParse(text, out float returnValue))
            returnValue = fallbackValue;

        return returnValue;
    }
}