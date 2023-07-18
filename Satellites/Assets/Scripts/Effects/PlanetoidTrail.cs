using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Planetoid))]
public class PlanetoidTrail : MonoBehaviour
{
    private Planetoid _planetoid;
    private LineRenderer _lineRenderer;

    private Queue <Vector3D> _positions;
    private float _previousTime;

    private void Awake()
    {
        _planetoid = GetComponent <Planetoid>();
        _lineRenderer = GetComponent <LineRenderer>();

        _lineRenderer.material = ViewManager.TrailMaterial;
        _lineRenderer.endWidth = 0;
        
        _lineRenderer.numCapVertices = 4;
        _lineRenderer.positionCount = 0;
        
        _positions = new Queue <Vector3D>();
        
        ViewManager.OnSelectPlanetoidStart.AddListener(
        planetoid =>
        {
            if (ViewManager.ShowLocalOrbits)
            {
                ClearLine();
                _lineRenderer.enabled = false;
            }
            
            SetOpacity(1);
        });
        
        ViewManager.OnSelectPlanetoidFinish.AddListener(
        planetoid =>
        {
            if (ViewManager.ShowOrbits && ViewManager.ShowLocalOrbits && planetoid != _planetoid)
                _lineRenderer.enabled = true;
        });

        ViewManager.OnShowOrbitChanged.AddListener(
        enable =>
        {
            if (!enable)
            {
                ClearLine();
                _lineRenderer.enabled = false;
            }
            else if (!ViewManager.AlignIsRunning && !(ViewManager.ShowLocalOrbits && ViewManager.TargetPlanetoid == _planetoid))
                _lineRenderer.enabled = true;
        });
        
        ViewManager.OnOrbitModeChanged.AddListener(
        enable =>
        {
            _positions.Clear();

            if (ViewManager.TargetPlanetoid == _planetoid)
                _lineRenderer.enabled = !enable;
        });
        
        ViewManager.OnZoomedIn.AddListener(SetOpacity);
    }

    void Start()
    {
        _previousTime = Time.time;
    }

    private void Update()
    {
        if (!_lineRenderer.enabled)
            return;
        
        Vector3 position = transform.position;
        _lineRenderer.startWidth = ViewManager.TrailWidthMult * Vector3.Distance(MainCamera.Camera.transform.position, position);
        
        if (!ViewManager.ShowLocalOrbits)
            SetPositions();
        
        if (_lineRenderer.positionCount > 0 && ViewManager.ShowLocalOrbits)
            _lineRenderer.SetPosition(0, position);
        
        if (_previousTime + ViewManager.TrailInterval > Time.time)
            return;
        
        _previousTime = Time.time;
        
        _positions.Enqueue(ViewManager.ShowLocalOrbits ? new Vector3D(position) : _planetoid.position);

        if (_positions.Count > ViewManager.TrailMaxPositionCount)
            _positions.Dequeue();

        if (ViewManager.ShowLocalOrbits)
            SetPositions();
    }

    private void SetPositions()
    {
        Vector3[] points = new Vector3[_positions.Count + 1];

        Vector3 offset = !ViewManager.ShowLocalOrbits ? ViewManager.ViewOffset.ToVector3 : Vector3.zero;
        
        for (int i = 1; i < points.Length; i++)
            points[i] = _positions.ElementAt(-i + _positions.Count).ToVector3 - offset;
        
        _lineRenderer.positionCount = points.Length;
        
        _lineRenderer.SetPositions(points);
        _lineRenderer.SetPosition(0, transform.position);
    }

    private void SetOpacity(float value)
    {
        Color color = new Color(1, 1, 1, value);

        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    private void ClearLine()
    {
        _positions.Clear();
        _lineRenderer.positionCount = 0;
    }
}