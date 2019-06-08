using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSkeletonRenderer : MonoBehaviour
{
    public GameObject LineRendererPrefab;
    public GameObject PointPrefab;

    private List<GameObject> _lineRendererGameObjects;
    private List<GameObject> _pointGameObjects;

    public void Draw(LevelSkeleton levelSkeleton)
    {
        if (_lineRendererGameObjects != null)
            _lineRendererGameObjects.ForEach(_ => Destroy(_));


        if (_pointGameObjects != null)
            _pointGameObjects.ForEach(_ => Destroy(_));

        _lineRendererGameObjects = new List<GameObject>();
        _pointGameObjects = new List<GameObject>();

        levelSkeleton.Lines.ToList().ForEach(_ =>
        {
            var lineRendererGameObject = CreateLine(_.Points.pointA.Position, _.Points.pointB.Position);
            _lineRendererGameObjects.Add(lineRendererGameObject);
        });

        levelSkeleton.Points.ToList().ForEach(_ =>
        {
            var pointGameObject = CreatePoint(_);
            _pointGameObjects.Add(pointGameObject);
        });
    }

    private GameObject CreateLine(Vector3 pointAPosition, Vector3 pointBPosition)
    {
        var lineRendererGameObject = Instantiate(LineRendererPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);

        LineRenderer lineRenderer = lineRendererGameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        lineRenderer.SetPosition(0, pointAPosition);
        lineRenderer.SetPosition(1, pointBPosition);

        return lineRendererGameObject;
    }

    private GameObject CreatePoint(SkeletonPoint point)
    {
        var pointPrefab = Instantiate(PointPrefab, point.Position, Quaternion.identity, gameObject.transform);
        var pointSpriteRenderer = pointPrefab.GetComponent<SpriteRenderer>();

        switch(point.Type)
        {
            case PointType.Initial:
                pointSpriteRenderer.color = Color.green;
                break;
            case PointType.Room:
                pointSpriteRenderer.color = Color.yellow;
                break;
            case PointType.Other:
                pointSpriteRenderer.color = Color.gray;
                break;
        }
        
        return pointPrefab;
    }
}
