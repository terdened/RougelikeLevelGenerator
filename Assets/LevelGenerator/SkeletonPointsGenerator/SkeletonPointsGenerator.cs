using System.Collections.Generic;
using UnityEngine;

public class SkeletonPointsGenerator : BaseGenerator<List<SkeletonPoint>, SkeletonPointsGeneratorParams>
{
    public SkeletonPointsGenerator(SkeletonPointsGeneratorParams generatorParams, IEnumerable<IGeneratorCriteria<List<SkeletonPoint>>> generatorCriteria = null) : base(generatorParams, generatorCriteria)
    {
    }

    protected override List<SkeletonPoint> Generate()
    {
        var result = new List<SkeletonPoint>();

        _params.InitialPoints?.ForEach(_ => result.Add(new SkeletonPoint(_, new EntityType(Color.green, "Initial"))));

        for (var i = 0; i < _params.RandomPointsCount; i++)
        {
            var criterias = new List<IGeneratorCriteria<SkeletonPoint>>
            {
                new AvoidNearSkeletonPointGeneratorCriteria(result)
            };

            var skeletonPointGeneratorParams = new SkeletonPointGeneratorParams
            {
                MaximalX = _params.MaximalX,
                MinimalX = _params.MinimalX,
                MaximalY = _params.MaximalY,
                MinimalY = _params.MinimalY
            };

            var skeletonPointGenerator = new SkeletonPointGenerator(skeletonPointGeneratorParams, criterias);
            var newPoint = skeletonPointGenerator.Execute();

            if (newPoint == null)
                return null;

            result.Add(newPoint);
        }

        return result;
    }
}
