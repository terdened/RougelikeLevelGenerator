using System;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonPoint
{
    public SkeletonPoint(Vector2 position, EntityType type = default)
    {
        Id = Guid.NewGuid().ToId();
        Position = position;
        Type = type;
    }
    public string Id { get; }

    public Vector2 Position { get; }

    public EntityType Type { get; set; }
}
