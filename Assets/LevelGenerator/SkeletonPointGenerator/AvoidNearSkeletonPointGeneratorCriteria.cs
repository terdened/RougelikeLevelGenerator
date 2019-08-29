﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvoidNearSkeletonPointGeneratorCriteria : IGeneratorCriteria<SkeletonPoint>
{
    private readonly List<SkeletonPoint> _initialPoints;

    public AvoidNearSkeletonPointGeneratorCriteria(List<SkeletonPoint> InitialPoints)
    {
        _initialPoints = InitialPoints;
    }

    public bool Verify(SkeletonPoint model)
    {
        if (_initialPoints.Any(_ => Vector2.Distance(_.Position, model.Position) < 1f))
            return false;

        return true;
    }
}
