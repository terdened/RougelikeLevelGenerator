using System.Collections.Generic;
using UnityEngine;

public class SkeletonPointGenerator : BaseGenerator<SkeletonPoint, SkeletonPointGeneratorParams>
{
    public SkeletonPointGenerator(SkeletonPointGeneratorParams generatorParams, IEnumerable<IGeneratorCriteria<SkeletonPoint>> generatorCriteria = null) : base(generatorParams, generatorCriteria)
    {
    }

    protected override SkeletonPoint Generate()
    {
        var randomPosition = new Vector2(Random.Range(_params.MinimalX, _params.MaximalX), Random.Range(_params.MinimalY, _params.MaximalY));
        return new SkeletonPoint(randomPosition, new EntityType(Color.yellow, "Room"));
    }
}
