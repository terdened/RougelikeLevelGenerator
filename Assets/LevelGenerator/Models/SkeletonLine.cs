using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class SkeletonLine
{
    public SkeletonLine(SkeletonPoint pointA, SkeletonPoint pointB, EntityType type = default)
    {
        Id = Guid.NewGuid().ToId();
        Points = (pointA, pointB);
        Type = type;
    }

    public string Id { get; }

    public float Length => Vector2.Distance(Points.pointA.Position, Points.pointB.Position);

    public bool ContainsSkeletonPoint(SkeletonPoint point) => Points.pointA == point || Points.pointB == point;

    public bool ContainsSkeletonPoint(Vector2 point) => Points.pointA.Position == point || Points.pointB.Position == point;

    public IReadOnlyCollection<SkeletonPoint> PointsList => new ReadOnlyCollection<SkeletonPoint>(new List<SkeletonPoint> { Points.pointA, Points.pointB });

    public (SkeletonPoint pointA, SkeletonPoint pointB) Points { get; }

    public EntityType Type { get; set; }
}
