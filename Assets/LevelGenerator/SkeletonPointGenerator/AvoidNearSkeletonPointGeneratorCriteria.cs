using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvoidNearSkeletonPointGeneratorCriteria : IGeneratorCriteria<SkeletonPoint>
{
    private readonly List<SkeletonPoint> _initialPoints;

    public AvoidNearSkeletonPointGeneratorCriteria(List<SkeletonPoint> initialPoints)
    {
        _initialPoints = initialPoints;
    }

    public bool Verify(SkeletonPoint model)
    {
        return !_initialPoints.Any(_ => Vector2.Distance(_.Position, model.Position) < 10f);
    }
}
