using UnityEngine;

public class SunFlare : MonoBehaviour
{
    [SerializeField] private float _baseBrightness;
    [SerializeField] private float _falloff;

    private LensFlare _flare;
        
    void Start()
    {
        _flare = GetComponent <LensFlare>();
    }

    void Update()
    {
        _flare.brightness = _baseBrightness + _falloff / Vector3.Distance(MainCamera.Camera.transform.position, transform.position);
    }
}