
using System.Collections.Generic;
using UnityEngine;

public class LevelSkeletonGeneratorParams
{
    public int AdditionalLinesCount { get; set; }

    public SkeletonPointsGeneratorParams SkeletonPointsGeneratorParams { get; set; }

    public bool ReplaceMoreThan45 { get; set; }

    public bool MergeNearBy { get; set; }

    public double MaxOpenSpacePerimeter { get; set; }

    public LevelSkeletonGeneratorParams()
    {
        ReplaceMoreThan45 = true;
        MergeNearBy = true;
        AdditionalLinesCount = 0;
        MaxOpenSpacePerimeter = 30;
        SkeletonPointsGeneratorParams = new SkeletonPointsGeneratorParams
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
