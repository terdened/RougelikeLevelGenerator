using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvoidNearSkeletonPointGeneratorCriteria : IGeneratorCriteria<List<SkeletonPoint>>
{
    public bool Verify(List<SkeletonPoint> model)
    {
        foreach (var point in model)
        {
            if (model.Any(_ => _ != point && Vector2.Distance(_.Position, point.Position) < 1f))
                return false;
        }

        return true;
    }
}
