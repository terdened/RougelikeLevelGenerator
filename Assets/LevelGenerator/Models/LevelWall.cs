using System;
using UnityEngine;

public class LevelWall
{
    private (Vector2 pointA, Vector2 pointB) _points;

    public LevelWall(Vector2 pointA, Vector2 pointB, EntityType type = default)
    {
        Id = Guid.NewGuid().ToId();
        _points = (pointA, pointB);
        Type = type;
    }

    public string Id { get; }

    public float Length => Vector2.Distance(_points.pointA, _points.pointB);
    
    public (Vector2 pointA, Vector2 pointB) Points => _points;

    public EntityType Type { get; set; }

    public void SetPointA(Vector2 pointA)
    {
        _points.pointA = pointA;
    }

    public void SetPointB(Vector2 pointB)
    {
        _points.pointB = pointB;
    }
}
