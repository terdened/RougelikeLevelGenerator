
using System.Collections.Generic;
using UnityEngine;

public class LevelSkeletonGeneratorParams
{
    public int AdditionalLinesCount { get; set; }

    public SkeletonPointsGeneratorParams SkeletonPointsGeneratorParams { get; set; }

    public bool ReplaceMoreThan45 { get; set; }

    public bool MergeNearBy { get; set; }

    public double MaxOpenSpacePerimeter { get; set; }
    public double MinOpenSpacePerimeter { get; set; }
    public List<(Vector2 a, Vector2 b, int doorIndex, int doorIndexTo, string scene)> InitialLines { get; set; }

    public LevelSkeletonGeneratorParams()
    {
        ReplaceMoreThan45 = true;
        MergeNearBy = true;
        AdditionalLinesCount = 0;
        MaxOpenSpacePerimeter = 45;
        MinOpenSpacePerimeter = 8;
        SkeletonPointsGeneratorParams = new SkeletonPointsGeneratorParams
        {
            RandomPointsCount = 25,
            MinimalX = -100,
            MaximalX = 100,
            MinimalY = -50,
            MaximalY = 50,
            InitialPoints = new List<Vector2> {
                new Vector2(110, 20),
                new Vector2(-110, -30)
            }
        };
        InitialLines = new List<(Vector2 a, Vector2 b, int doorIndex, int doorIndexTo, string scene)>
        {
            (new Vector2(115, 20), new Vector2(110, 20), 1, 0, "ForgottenOneScene"),
            (new Vector2(-115, -30), new Vector2(-110, -30), 0, 0, "HubScene")
        };
    }
}
