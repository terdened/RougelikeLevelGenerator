using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelRenderer : MonoBehaviour
{
    public GameObject LineRendererPrefab;
    public GameObject PointPrefab;

    private List<GameObject> _lineRendererGameObjects;
    private List<GameObject> _roomRendererGameObjects;

    public void Draw(Level level)
    {
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
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material.color = type.Color;

        lineRenderer.SetPosition(0, pointAPosition);
        lineRenderer.SetPosition(1, pointBPosition);

        return lineRendererGameObject;
    }
}
