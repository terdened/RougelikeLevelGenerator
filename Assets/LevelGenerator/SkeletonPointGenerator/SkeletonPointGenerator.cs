using System.Collections.Generic;
using UnityEngine;

public class SkeletonPointGenerator : BaseGenerator<List<SkeletonPoint>, SkeletonPointGeneratorParams>
{
    public SkeletonPointGenerator(SkeletonPointGeneratorParams generatorParams, IEnumerable<IGeneratorCriteria<List<SkeletonPoint>>> generatorCriteria = null) : base(generatorParams, generatorCriteria)
    {
    }

    protected override List<SkeletonPoint> Generate()
    {
        var result = new List<SkeletonPoint>();

        _params.InitialPoints?.ForEach(_ => result.Add(new SkeletonPoint(_, PointType.Initial)));

        for (var i = 0; i < _params.RandomPointsCount; i++)
        {
            var randomPosition = new Vector2(Random.Range(_params.MinimalX, _params.MaximalX), Random.Range(_params.MinimalY, _params.MaximalY));
            result.Add(new SkeletonPoint(randomPosition, PointType.Room));
        }

        return result;
    }
}
