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

        if (Params.LevelSkeleton?.Lines != null)
        {
            var isChanges = true;

            while (isChanges)
            {
                isChanges = false;
                var duplicate = new List<SkeletonLine>();
                foreach(var line in Params.LevelSkeleton.Lines)
                {
                    var duplicates = Params.LevelSkeleton.Lines.Where(l => line != l)
                        .Where(l => l.ContainsSkeletonPoint(line.Points.pointA.Position) 
                                    && l.ContainsSkeletonPoint(line.Points.pointB.Position));

                    if (duplicate.Count <= 0) continue;

                    duplicate.AddRange(duplicates);
                    break;

                }

                if (duplicate.Count <= 0) continue;

                Params.LevelSkeleton.RemoveLines(duplicate);
                isChanges = true;
            }

            var skeletonLinesWithWalls = new List<(SkeletonLine line, LevelWall wallA, LevelWall wallB)>();
            foreach (var line in Params.LevelSkeleton.Lines)
            {
                var walls = line.GetLevelWalls().ToArray();

                var wallA = walls.Any(_ => _.Type == EntityTypeConstants.Unknown) ? walls[0] : null;
                var wallB = walls.Count(_ => _.Type == EntityTypeConstants.Unknown) > 1 ? walls[1] : null;

                skeletonLinesWithWalls.Add((line, wallA, wallB));
                level.AddWalls(walls);
            }

            foreach (var point in Params.LevelSkeleton.Points.Where(_ => Params.LevelSkeleton.LinesForPoint(_).Count >= 2))
            {
                var skeletonLinesWithWallsForPoint = skeletonLinesWithWalls
                    .Where(_ => _.line.ContainsSkeletonPoint(point))
                    .Select(_ => (
                        line: new SkeletonLine(
                            point == _.line.Points.pointA 
                                    ? _.line.Points.pointA 
                                    : _.line.Points.pointB, 
                            point == _.line.Points.pointA 
                                ? _.line.Points.pointB 
                                : _.line.Points.pointA),
                        _.wallA, 
                        _.wallB))
                    .ToArray();

                skeletonLinesWithWalls.ForEach(_ =>
                {
                    if (_.wallA != null && Vector2.Distance(point.Position, _.wallA.Points.pointB) < Vector2.Distance(point.Position, _.wallA.Points.pointA))
                    {
                        var pointA = _.wallA.Points.pointA;
                        _.wallA.SetPointA(_.wallA.Points.pointB);
                        _.wallA.SetPointB(pointA);
                    }

                    if (_.wallB != null && Vector2.Distance(point.Position, _.wallB.Points.pointB) < Vector2.Distance(point.Position, _.wallB.Points.pointA))
                    {
                        var pointA = _.wallB.Points.pointA;
                        _.wallB.SetPointA(_.wallB.Points.pointB);
                        _.wallB.SetPointB(pointA);
                    }
                });

                if (skeletonLinesWithWallsForPoint.Count() < 2)
                    continue;

                skeletonLinesWithWallsForPoint = skeletonLinesWithWallsForPoint.OrderBy(_ => GetAngle(point.Position, _.line.Points.pointB.Position)).ToArray();

                for(var i = 0; i < skeletonLinesWithWallsForPoint.Count(); i++)
                {
                    var angle = GetAngle(point.Position, skeletonLinesWithWallsForPoint[i].line.Points.pointB.Position);
                    var wallAAngle = skeletonLinesWithWallsForPoint[i].wallA != null 
                        ? GetAngle(point.Position, skeletonLinesWithWallsForPoint[i].wallA.Points.pointB) 
                        : (float?)null;

                    if (wallAAngle.HasValue)
                    {
                        if (Mathf.Abs(wallAAngle.Value - angle) > 180)
                        {
                            if (wallAAngle > angle)
                                wallAAngle -= 360;
                            else
                                angle -= 360;
                        }

                        if (!(angle >= wallAAngle)) continue;

                        var wall = skeletonLinesWithWallsForPoint[i].wallB;
                        skeletonLinesWithWallsForPoint[i].wallB = skeletonLinesWithWallsForPoint[i].wallA;
                        skeletonLinesWithWallsForPoint[i].wallA = wall;
                    } else
                    {
                        var wallBAngle = skeletonLinesWithWallsForPoint[i].wallA != null ? GetAngle(point.Position, skeletonLinesWithWallsForPoint[i].wallB.Points.pointB) : (float?)null;

                        if (!wallBAngle.HasValue) continue;

                        if (Mathf.Abs(wallBAngle.Value - angle) > 180)
                        {
                            if (wallBAngle > angle)
                                wallBAngle -= 360;
                            else
                                angle -= 360;
                        }

                        if (!(angle < wallBAngle)) continue;

                        var wall = skeletonLinesWithWallsForPoint[i].wallB;
                        skeletonLinesWithWallsForPoint[i].wallB = skeletonLinesWithWallsForPoint[i].wallA;
                        skeletonLinesWithWallsForPoint[i].wallA = wall;
                    }

                }

                for (var i = 0; i < skeletonLinesWithWallsForPoint.Count(); i++)
                {
                    var wallA = skeletonLinesWithWallsForPoint[i].wallA;
                    var wallB = i < skeletonLinesWithWallsForPoint.Count() - 1
                                    ? skeletonLinesWithWallsForPoint[i + 1].wallB
                                    : skeletonLinesWithWallsForPoint[0].wallB;

                    if (wallA == null || wallB == null) continue;

                    var intersection = wallA.FindIntersection(wallB);

                    if (intersection.HasValue)
                    {
                        wallA.SetPointA(intersection.Value);
                        wallB.SetPointA(intersection.Value);
                    }
                    else if (Mathf.Abs(wallA.Points.pointA.x - wallB.Points.pointA.x) < 0.01 || Mathf.Abs(wallA.Points.pointA.y - wallB.Points.pointA.y) < 0.01)
                    {
                        var middlePoint = new Vector2((wallA.Points.pointA.x + wallB.Points.pointA.x) / 2, (wallA.Points.pointA.y + wallB.Points.pointA.y) / 2);
                        wallA.SetPointA(middlePoint);
                        wallB.SetPointA(middlePoint);
                    }
                }
            }

            foreach (var line in Params.LevelSkeleton.Lines)
            {
                var elevator = line.GetLevelElevator();

                if (elevator != null)
                    level.AddWall(elevator);
            }
        }

        if (Params.LevelSkeleton?.Lines != null)
        {
            level.AddRooms(Params.LevelSkeleton.Points.Select(_ => _.Position));
        }

        return level;
    }

    private static float GetAngle(Vector2 vectorA, Vector2 vectorB)
    {
        var diffX = vectorB.x - vectorA.x;
        var diffY = vectorB.y - vectorA.y;

        var result = Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg;
        return result;
    }
}
