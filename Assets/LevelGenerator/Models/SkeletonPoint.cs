using System;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonPoint
{
    private Vector2 _position;
    private EntityType _type;

    private readonly List<SkeletonLine> _lines;

    public SkeletonPoint(Vector2 position, EntityType type = null)
    {
        Id = Guid.NewGuid().ToId();
        _position = position;
        _type = type ?? new EntityType();
        _lines = new List<SkeletonLine>();
    }
    public string Id { get; }

    public Vector2 Position => _position;
    public EntityType Type
    {
        get => _type;

        set
        {
            _type = value;
        }
    }

    public List<SkeletonLine> Lines => _lines;

    public void AddLine(SkeletonLine line)
    {
        _lines.Add(line);
    }

    public void RemoveLine(SkeletonLine line)
    {
        _lines.Remove(line);
    }
}
