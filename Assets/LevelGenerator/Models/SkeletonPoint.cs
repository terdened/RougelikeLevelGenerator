using System;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonPoint
{
    private Vector2 _position;
    private EntityType _type;

    public SkeletonPoint(Vector2 position, EntityType type = default)
    {
        Id = Guid.NewGuid().ToId();
        _position = position;
        _type = type;
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
}
