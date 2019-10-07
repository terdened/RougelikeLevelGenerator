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

        // 1) Convert SkeletonLines in walls and air platforms
        if (Params.LevelSkeleton?.Lines != null)
        {
            var skeletonLinesWithWalls = new List<(SkeletonLine line, LevelWall wallA, LevelWall wallB)>();
            var skeletonLinesWithAirPlatforms = new List<(SkeletonLine line, LevelWall wallA, LevelWall wallB)>();

            foreach (var line in Params.LevelSkeleton.Lines)
            {
                var walls = line.GetLevelWalls().ToArray();

                var wallA = walls.FirstOrDefault(_ => _.Type == EntityTypeConstants.Unknown);
                var wallB = walls.Count(_ => _.Type == EntityTypeConstants.Unknown) > 1 
                    ? walls.Where(_ => _.Type == EntityTypeConstants.Unknown).Skip(1).First() 
                    : null;

                skeletonLinesWithWalls.Add((line, wallA, wallB));

                var airPlatformA = walls.FirstOrDefault(_ => _.Type == EntityTypeConstants.AirPlatform);
                skeletonLinesWithAirPlatforms.Add((line, airPlatformA, null));

                level.AddWalls(walls);
            }

            MergeWalls(level, skeletonLinesWithWalls);
            MergeWalls(level, skeletonLinesWithAirPlatforms);
        }

        // 2) Convert SkeletonLines to Elevators

        if (Params.LevelSkeleton?.Lines != null)
        {
            foreach (var line in Params.LevelSkeleton.Lines)
            {
                var elevator = line.GetLevelElevator();

                if (elevator != null)
                    level.AddElevator(elevator);
            }
        }

        // 3) Generate Rooms


        return level;
    }

    private void MergeWalls(Level level, List<(SkeletonLine line, LevelWall wallA, LevelWall wallB)> skeletonLinesWithWalls)
    {
        foreach (var point in Params.LevelSkeleton.Points.Where(_ => Params.LevelSkeleton.LinesForPoint(_).Count >= 1))
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

            if (skeletonLinesWithWallsForPoint.Count() == 1)
            {
                if (skeletonLinesWithWallsForPoint.First().wallA != null &&
                    skeletonLinesWithWallsForPoint.First().wallB != null)
                {
                    level.AddWall(new LevelWall(skeletonLinesWithWallsForPoint.First().wallA.Points.pointA,
                        skeletonLinesWithWallsForPoint.First().wallB.Points.pointA,
                        EntityTypeConstants.Deadlock));
                }
            }

            if (skeletonLinesWithWallsForPoint.Count() < 2)
                continue;

            skeletonLinesWithWallsForPoint = skeletonLinesWithWallsForPoint.OrderBy(_ => point.Position.GetAngle(_.line.Points.pointB.Position)).ToArray();

            for (var i = 0; i < skeletonLinesWithWallsForPoint.Count(); i++)
            {
                var angle = point.Position.GetAngle(skeletonLinesWithWallsForPoint[i].line.Points.pointB.Position);
                var wallAAngle = skeletonLinesWithWallsForPoint[i].wallA != null
                    ? point.Position.GetAngle(skeletonLinesWithWallsForPoint[i].wallA.Points.pointB)
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
                }
                else
                {
                    var wallBAngle = skeletonLinesWithWallsForPoint[i].wallA != null ? point.Position.GetAngle(skeletonLinesWithWallsForPoint[i].wallB.Points.pointB) : (float?)null;

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
    }
}
