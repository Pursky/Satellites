using UnityEngine;

public class PlanetoidRotation : MonoBehaviour
{
    [SerializeField, Tooltip("In degrees/100000 s")] private float _rotationSpeed;
    
    [Space]
    [SerializeField, Tooltip("In days")] private float _rotationPeriod;

    void Update()
    {
        transform.localEulerAngles += Vector3.up * (_rotationSpeed * Time.deltaTime * (float)PhysicsManager.TimeScale);
    }

    private void OnValidate()
    {
        _rotationSpeed = 1/(_rotationPeriod  * 0.00240833333f);
    }
}