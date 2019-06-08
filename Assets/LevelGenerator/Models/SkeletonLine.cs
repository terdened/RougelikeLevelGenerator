using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class SkeletonLine
{
    private (SkeletonPoint pointA, SkeletonPoint pointB) _points;

    public SkeletonLine(SkeletonPoint pointA, SkeletonPoint pointB)
    {
        Id = Guid.NewGuid().ToId();
        _points = (pointA, pointB);
    }

    public string Id { get; }

    public float Length => Vector2.Distance(_points.pointA.Position, _points.pointB.Position);

    public IReadOnlyCollection<SkeletonPoint> PointsList => new ReadOnlyCollection<SkeletonPoint>(new List<SkeletonPoint> { _points.pointA, _points.pointB });

    public (SkeletonPoint pointA, SkeletonPoint pointB) Points => _points;
}
