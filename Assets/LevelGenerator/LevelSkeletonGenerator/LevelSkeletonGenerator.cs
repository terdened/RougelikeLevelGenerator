using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSkeletonGenerator : BaseGenerator<LevelSkeleton, LevelSkeletonGeneratorParams>
{
    private readonly SkeletonPointGenerator _skeletonPointGenerator;

    public LevelSkeletonGenerator(LevelSkeletonGeneratorParams generatorParams, IEnumerable<IGeneratorCriteria<LevelSkeleton>> generatorCriteria = null) : base(generatorParams, generatorCriteria)
    {
        var pointGeneratorParams = new SkeletonPointGeneratorParams
        {
            RandomPointsCount = 25,
            MinimalX = -10,
            MaximalX = 10,
            MinimalY = -5,
            MaximalY = 5,
            InitialPoints = new List<Vector2> {
                new Vector2(11, 2),
                new Vector2(-11, -3),
                new Vector2(-11, 5)
            }
        };

        var criterias = new List<IGeneratorCriteria<List<SkeletonPoint>>>
        {
            new AvoidNearSkeletonPointGeneratorCriteria()
        };

        _skeletonPointGenerator = new SkeletonPointGenerator(pointGeneratorParams, criterias);
    }

    protected override LevelSkeleton Generate()
    {
        var result = new LevelSkeleton();

        var points = _skeletonPointGenerator.Execute();

        result.AddPoints(points);

        var fullSkeletonLines = result.ToGraph().ToFullGraph().Edges.ToList().Select(_ =>
        {
            return new SkeletonLine(_.Vertexes.ToList()[0].Data, _.Vertexes.ToList()[1].Data);
        });

        var fullSkeleton = new LevelSkeleton();
        fullSkeleton.AddLines(fullSkeletonLines);

        var newLines = fullSkeleton.ToGraph().AlgorithmByPrim().Edges.ToList().Select(_ =>
        {
            return new SkeletonLine(_.Vertexes.ToList()[0].Data, _.Vertexes.ToList()[1].Data);
        });

        result.AddLines(newLines);

        return result;
    }
}
