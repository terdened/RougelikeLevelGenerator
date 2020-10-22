using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelMapRenderer : MonoBehaviour
{
    public GameObject LineRendererPrefab;
    public GameObject PointPrefab;

    private List<GameObject> _lineRendererGameObjects;
    private List<GameObject> _roomRendererGameObjects;
    private float _lineWidth;

    public void Draw(Level level, float lineWidth = 0.5f)
    {
        _lineWidth = lineWidth;
        Clear();
        _lineRendererGameObjects = new List<GameObject>();
        _roomRendererGameObjects = new List<GameObject>();

        level.Walls.ToList().ForEach(_ =>
        {
            var lineRendererGameObject = CreateLine(_.Points.pointA, _.Points.pointB, _.Type, _.Id);
            _lineRendererGameObjects.Add(lineRendererGameObject);
        });

        level.Elevators.ToList().ForEach(_ =>
        {
            var lineRendererGameObject = CreateLine(_.Points.pointA, _.Points.pointB, _.Type, _.Id);
            _lineRendererGameObjects.Add(lineRendererGameObject);
        });
    }

    public void Clear()
    {
        _lineRendererGameObjects?.ForEach(Destroy);
        _roomRendererGameObjects?.ForEach(Destroy);
    }

    private GameObject CreateLine(Vector3 pointAPosition, Vector3 pointBPosition, EntityType type, string entityId)
    {
        var lineRendererGameObject = Instantiate(LineRendererPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);

        var idHolder = lineRendererGameObject.AddComponent<EntityIdHolder>();
        idHolder.SetId(entityId);

        var lineRenderer = lineRendererGameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = _lineWidth;
        lineRenderer.endWidth = _lineWidth;
        lineRenderer.material.color = type.Color;

        lineRenderer.SetPosition(0, pointAPosition);
        lineRenderer.SetPosition(1, pointBPosition);

        return lineRendererGameObject;
    }
}
