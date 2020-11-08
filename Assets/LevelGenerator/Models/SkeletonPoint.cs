using System;
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

    public int? Index { get; set; }

    public int? IndexTo { get; set; }

    public string Scene { get; set; }

    public EntityType Type { get; set; }
}
