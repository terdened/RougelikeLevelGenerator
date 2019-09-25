using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class SkeletonLine
{
    private EntityType _type;

    private (SkeletonPoint pointA, SkeletonPoint pointB) _points;

    public SkeletonLine(SkeletonPoint pointA, SkeletonPoint pointB, EntityType type = null)
    {
        Id = Guid.NewGuid().ToId();
        _points = (pointA, pointB);
        _type = type ?? new EntityType();
    }

    public string Id { get; }

    public float Length => Vector2.Distance(_points.pointA.Position, _points.pointB.Position);

    public bool ContainsSkeletonPoint(SkeletonPoint point) => _points.pointA == point || _points.pointB == point;

    public bool ContainsSkeletonPoint(Vector2 point) => _points.pointA.Position == point || _points.pointB.Position == point;

    public IReadOnlyCollection<SkeletonPoint> PointsList => new ReadOnlyCollection<SkeletonPoint>(new List<SkeletonPoint> { _points.pointA, _points.pointB });

    public (SkeletonPoint pointA, SkeletonPoint pointB) Points => _points;

    public EntityType Type {
        get => _type;

        set
        {
            _type = value;
        }
    }
}
