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

        Params.InitialPoints?.ForEach(_ => result.Add(new SkeletonPoint(_, new EntityType(Color.green, "Initial"))));

        for (var i = 0; i < Params.RandomPointsCount; i++)
        {
            var criteria = new List<IGeneratorCriteria<SkeletonPoint>>
            {
                new AvoidNearSkeletonPointGeneratorCriteria(result)
            };

            var skeletonPointGeneratorParams = new SkeletonPointGeneratorParams
            {
                MaximalX = Params.MaximalX,
                MinimalX = Params.MinimalX,
                MaximalY = Params.MaximalY,
                MinimalY = Params.MinimalY
            };

            var skeletonPointGenerator = new SkeletonPointGenerator(skeletonPointGeneratorParams, criteria);
            var newPoint = skeletonPointGenerator.Execute();

            if (newPoint == null)
                return null;

            result.Add(newPoint);
        }

        return result;
    }
}
