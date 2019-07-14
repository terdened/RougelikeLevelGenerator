
using System.Collections.Generic;
using UnityEngine;

public class LevelSkeletonGeneratorParams
{
    public int AdditionalLinesCount { get; set; }

    public SkeletonPointGeneratorParams SkeletonPointGeneratorParams { get; set; }

    public LevelSkeletonGeneratorParams()
    {
        AdditionalLinesCount = 9;
        SkeletonPointGeneratorParams = new SkeletonPointGeneratorParams
        {
            RandomPointsCount = 25,
            MinimalX = -10,
            MaximalX = 10,
            MinimalY = -5,
            MaximalY = 5,
            InitialPoints = new List<Vector2> {
                new Vector2(11, 2),
                new Vector2(-11, -3)
            }
        };
    }
}
