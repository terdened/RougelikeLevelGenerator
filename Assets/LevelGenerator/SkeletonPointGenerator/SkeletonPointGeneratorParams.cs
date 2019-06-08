
using System.Collections.Generic;
using UnityEngine;

public class SkeletonPointGeneratorParams
{
    public List<Vector2> InitialPoints { get; set; }

    public int RandomPointsCount { get; set; }

    public float MinimalX { get; set; }
    public float MaximalX { get; set; }
    public float MinimalY { get; set; }
    public float MaximalY { get; set; }
}
