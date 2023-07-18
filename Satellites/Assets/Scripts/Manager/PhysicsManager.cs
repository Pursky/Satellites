using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 1 unit of space  =   100000km
// 1 unit of time   =   100000s
// 1 unit of speed  =   1 km/s
// 1 unit of mass   =   1 kg^24

[DefaultExecutionOrder(-10)]
public class PhysicsManager : MonoBehaviour
{
    public const double GravitationalConstant = 0.667430;
    private static PhysicsManager s_instance;
    
    [SerializeField] private double _timeScale = 1;
    
    [Space]
    [SerializeField] private double _timeScaleMin;
    [SerializeField] private double _timeScaleMax;

    private List <Planetoid> _planetoids;
    private UnityEvent <Planetoid> _planetoidWasAdded;

    public static double TimeScale
    {
        get => s_instance._timeScale;
        set => s_instance._timeScale = value;
    }

    public static double TimeScaleMin => s_instance._timeScaleMin;
    public static double TimeScaleMax => s_instance._timeScaleMax;

    public static Planetoid[] Planetoids => s_instance._planetoids.ToArray();

    public static UnityEvent <Planetoid> PlanetoidWasAdded => s_instance._planetoidWasAdded;

    private void Awake()
    {
        if (!s_instance)
            s_instance = this;
        else
        {
            Destroy(this);
            return;
        }

        _planetoids = new List <Planetoid>();
        _planetoidWasAdded = new UnityEvent <Planetoid>();
    }

    private void Start()
    {
        _planetoids.Sort();

        for (int i = 0; i < _planetoids.Count; i++)
        {
            _planetoids[i].SetupPosition();
            _planetoids[i].index = i;
        }
    }

    void Update()
    {
        if (_planetoids.Count == 0)
            return;

        for (int i = 0; i < _planetoids.Count; i++)
        {
            for (int j = i; j < _planetoids.Count; j++)
            {
                if (i == j)
                    continue;

                Planetoid a = _planetoids[i];
                Planetoid b = _planetoids[j];

                Vector3D aToB = b.position - a.position;
                Vector3D aToBDirection = aToB.Normalized;
                
                double force = GravitationalConstant * ((a.mass * b.mass) / Math.Pow(aToB.Magnitude, 2));
                
                a.AddForce(aToBDirection * force);
                b.AddForce(aToBDirection * -force);
            }
        }
    }

    private void OnValidate()
    {
        _timeScale = Math.Clamp(_timeScale, _timeScaleMin, _timeScaleMax);
    }

    public static void AddPlanetoid(Planetoid planetoid)
    {
        s_instance._planetoids.Add(planetoid);
    }
    
    public static void RemovePlanetoid(Planetoid planetoid)
    {
        s_instance._planetoids.Remove(planetoid);
    }

    public static int GetIndex(Planetoid planetoid) => s_instance._planetoids.IndexOf(planetoid);
    
}

[Serializable]
public struct Vector3D
{
    public double x;
    public double y;
    public double z;
    
    public Vector3D(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3D(Vector3 vec)
    {
        x = vec.x;
        y = vec.y;
        z = vec.z;
    }
    
    public Vector3D Abs => new(Math.Abs(x), Math.Abs(y), Math.Abs(z));
    public double Magnitude => Math.Sqrt(x * x + y * y + z * z);
    public Vector3D Normalized => this / Magnitude;
    public double Distance(Vector3D other) => (this - other).Abs.Magnitude;
    public Vector3 ToVector3 => new Vector3((float)x, (float)y, (float)z);
    
    public override string ToString() => $"({x} | {y} | {z})";

    public static Vector3D Zero => new(0, 0, 0);
    public static Vector3D One => new(1, 1, 1);
    
    public static Vector3D Forward => new(0, 0, 1);
    public static Vector3D Right => new(1, 0, 0);
    public static Vector3D Up => new(0, 1, 0);

    public static Vector3D Lerp(Vector3D a, Vector3D b, float time) => a + (b - a) * time;

    public static Vector3D operator +(Vector3D a, Vector3D b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
    public static Vector3D operator -(Vector3D a, Vector3D b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
    public static Vector3D operator -(Vector3D a) => a * -1;
    public static Vector3D operator *(Vector3D a, double b) => new(a.x * b, a.y * b, a.z * b);
    public static Vector3D operator *(double a, Vector3D b) => new(a * b.x, a * b.y, a * b.z);
    public static Vector3D operator /(Vector3D a, double b) => new(a.x / b, a.y / b, a.z / b);
}