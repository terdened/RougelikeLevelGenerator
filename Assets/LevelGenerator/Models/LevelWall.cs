using System;
using UnityEngine;

public class LevelWall
{
    private EntityType _type;

    private (Vector2 pointA, Vector2 pointB) _points;

    public LevelWall(Vector2 pointA, Vector2 pointB, EntityType type = null)
    {
        Id = Guid.NewGuid().ToId();
        _points = (pointA, pointB);
        _type = type ?? new EntityType();
    }

    public string Id { get; }

    public float Length => Vector2.Distance(_points.pointA, _points.pointB);
    
    public (Vector2 pointA, Vector2 pointB) Points => _points;

    public EntityType Type
    {
        get => _type;

        set
        {
            _type = value;
        }
    }
}
