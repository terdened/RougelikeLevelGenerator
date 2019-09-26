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
    }

    public void Clear()
    {
        if (_lineRendererGameObjects != null)
            _lineRendererGameObjects.ForEach(_ => Destroy(_));
        
        if (_roomRendererGameObjects != null)
            _roomRendererGameObjects.ForEach(_ => Destroy(_));
    }

    private GameObject CreateLine(Vector3 pointAPosition, Vector3 pointBPosition, EntityType type, string entityId)
    {
        var lineRendererGameObject = Instantiate(LineRendererPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);

        EntityIdHolder idHolder = lineRendererGameObject.AddComponent<EntityIdHolder>();
        idHolder.SetId(entityId);

        LineRenderer lineRenderer = lineRendererGameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material.color = type.Color;

        lineRenderer.SetPosition(0, pointAPosition);
        lineRenderer.SetPosition(1, pointBPosition);

        return lineRendererGameObject;
    }

    private GameObject CreatePoint(Vector2 room)
    {
        var pointPrefab = Instantiate(PointPrefab, room, Quaternion.identity, gameObject.transform);
        pointPrefab.transform.localScale = new Vector3(3,3,3);
        var pointSpriteRenderer = pointPrefab.GetComponent<SpriteRenderer>();
        pointSpriteRenderer.color = Color.gray;

        return pointPrefab;
    }

    private GameObject CreatePoint2(Vector2 room)
    {
        var pointPrefab = Instantiate(PointPrefab, room, Quaternion.identity, gameObject.transform);
        pointPrefab.transform.localScale = new Vector3(2f, 2f, 2f);
        pointPrefab.transform.Translate(new Vector3(0,0,-1));
        var pointSpriteRenderer = pointPrefab.GetComponent<SpriteRenderer>();
        pointSpriteRenderer.color = new Color(0.35f, 0.35f, 0.35f);

        return pointPrefab;
    }
}
