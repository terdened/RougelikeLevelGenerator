using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : BaseGenerator<Level, LevelGeneratorParams>
{
    public LevelGenerator(LevelGeneratorParams generatorParams, IEnumerable<IGeneratorCriteria<Level>> generatorCriteria = null) : base(generatorParams, generatorCriteria)
    {
    }

    protected override Level Generate()
    {
        var level = new Level();

        if (_params.LevelSkeleton != null && _params.LevelSkeleton.Lines != null)
        {
            foreach (var line in _params.LevelSkeleton.Lines)
            {
                var walls = line.GetLevelWalls();

                if(walls != null)
                    level.AddWalls(walls);
            }

            //foreach (var wall in level.Walls.Where(_ => _.Type.Name == "Unknown"))
            //{
            //    var otherWalls = level.Walls.Where(_ => _.Type.Name == "Unknown").Where(_ => _.Id != wall.Id).ToList();

            //    var allIntersectionsForPointA = otherWalls.Select(_ =>
            //    {
            //        var intersection = _.FindIntersection(wall);
            //        return (Intersection: intersection, Distance: intersection.HasValue ? Vector2.Distance(intersection.Value, wall.Points.pointA) : (float?)null);
            //    });

            //    var intersectionForPointA = allIntersectionsForPointA.Where(_ => _.Distance.HasValue).OrderBy(_ => _.Distance).First();

            //    if (intersectionForPointA.Distance < 0.3f)
            //        wall.SetPointA(intersectionForPointA.Intersection.Value);

            //    var allIntersectionsForPointB = otherWalls.Select(_ =>
            //    {
            //        var intersection = _.FindIntersection(wall);
            //        return (Intersection: intersection, Distance: intersection.HasValue ? Vector2.Distance(intersection.Value, wall.Points.pointB) : (float?)null);
            //    });

            //    var firstIntersectionForPointB = allIntersectionsForPointB.Where(_ => _.Distance.HasValue).OrderBy(_ => _.Distance).First().Intersection;

            //    var intersectionForPointB = allIntersectionsForPointB.Where(_ => _.Distance.HasValue).OrderBy(_ => _.Distance).First();

            //    if (intersectionForPointB.Distance < 0.3f)
            //        wall.SetPointB(intersectionForPointB.Intersection.Value);
            //}

            //foreach (var wall in level.Walls.Where(_ => _.Type.Name == "Air Plartform"))
            //{
            //    var otherWalls = level.Walls.Where(_ => _.Type.Name == "Air Plartform").Where(_ => _.Id != wall.Id).ToList();

            //    var allIntersectionsForPointA = otherWalls.Select(_ =>
            //    {
            //        var intersection = _.FindIntersection(wall);
            //        return (Intersection: intersection, Distance: intersection.HasValue ? Vector2.Distance(intersection.Value, wall.Points.pointA) : (float?)null);
            //    });

            //    var intersectionForPointA = allIntersectionsForPointA.Where(_ => _.Distance.HasValue).OrderBy(_ => _.Distance).First();

            //    if (intersectionForPointA.Distance < 0.3f)
            //        wall.SetPointA(intersectionForPointA.Intersection.Value);

            //    var allIntersectionsForPointB = otherWalls.Select(_ =>
            //    {
            //        var intersection = _.FindIntersection(wall);
            //        return (Intersection: intersection, Distance: intersection.HasValue ? Vector2.Distance(intersection.Value, wall.Points.pointB) : (float?)null);
            //    });

            //    var intersectionForPointB = allIntersectionsForPointB.Where(_ => _.Distance.HasValue).OrderBy(_ => _.Distance).First();

            //    if (intersectionForPointB.Distance < 2f)
            //        wall.SetPointB(intersectionForPointB.Intersection.Value);
            //}

            foreach (var line in _params.LevelSkeleton.Lines)
            {
                var elevator = line.GetLevelElevator();

                if (elevator != null)
                    level.AddWall(elevator);
            }
        }

        if (_params.LevelSkeleton != null && _params.LevelSkeleton.Lines != null)
        {
            level.AddRooms(_params.LevelSkeleton.Points.Select(_ => _.Position));
        }

        return level;
    }
}
