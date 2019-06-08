using System;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonPoint
{
    private Vector2 _position;

    private PointType _type;

    private readonly List<SkeletonLine> _lines;

    public SkeletonPoint(Vector2 position, PointType type)
    {
        Id = Guid.NewGuid().ToId();
        _position = position;
        _type = type;
        _lines = new List<SkeletonLine>();
    }
    public string Id { get; }

    public Vector2 Position => _position;
    public PointType Type => _type;

    public void AddLine(SkeletonLine line)
    {
        _lines.Add(line);
    }

    public void RemoveLine(SkeletonLine line)
    {
        _lines.Remove(line);
    }
}
