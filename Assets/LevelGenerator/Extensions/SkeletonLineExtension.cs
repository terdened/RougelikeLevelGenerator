using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class SkeletonLineExtension
{
    public static float? GetDistanceBetweenLineAndPoint(this SkeletonLine line, SkeletonPoint point)
        => line.ToVector2().GetDistanceBetweenLineAndPoint(point.Position);

    public static double GetLineAngle(this SkeletonLine line)
        => line.ToVector2().GetLineAngle();

    public static double GetOrigianlLineAngle(this SkeletonLine line)
        => line.ToVector2().GetOrigianlLineAngle();

    public static SkeletonPoint GetMiddlePoint(this SkeletonLine line)
        => new SkeletonPoint(line.ToVector2().GetMiddlePoint());

    public static bool IsSkeletonPointInside(this List<SkeletonLine> perimeter, SkeletonPoint point)
    {
        var randomAngle = Random.Range(0f, 1.5708f);
        var b = point.Position.y - Mathf.Tan(randomAngle) * point.Position.x;
        var secondPoint = new SkeletonPoint(new Vector2(1000, Mathf.Tan(randomAngle) * 1000 + b));
        var ray = new SkeletonLine(point, secondPoint);

        while (perimeter.Any(_ => (ray.GetDistanceBetweenLineAndPoint(_.Points.pointA) == -1 ? 0 : ray.GetDistanceBetweenLineAndPoint(_.Points.pointA)) < 0.1 || (ray.GetDistanceBetweenLineAndPoint(_.Points.pointB) == -1 ? 0 : ray.GetDistanceBetweenLineAndPoint(_.Points.pointB)) < 0.1))
        {
            randomAngle = Random.Range(0, 90);
            b = point.Position.y - Mathf.Tan(randomAngle) * point.Position.x;
            secondPoint = new SkeletonPoint(new Vector2(1000, Mathf.Tan(randomAngle) * 1000 + b));
            ray = new SkeletonLine(point, secondPoint);
        }

        var intesectionsCount = perimeter.Where(_ => _.FindIntersection(ray).HasValue).Count();

        return intesectionsCount % 2 == 0 ? false : true;
    }

    public static bool IsSkeletonPointBelongs(this List<SkeletonLine> perimeter, SkeletonPoint point)
    {
        if (perimeter.Any(l => l.ContainsSkeletonPoint(point)))
            return true;

        return perimeter.IsSkeletonPointInside(point);
    }

    public static double GetPathLength(this List<SkeletonLine> path) => path.Sum(_ => _.Length);

    public static bool IsCycleEquals(this List<SkeletonLine> cycleA, List<SkeletonLine> cycleB)
    {
        if (cycleA.Count() != cycleB.Count())
            return false;

        var result = true;

        cycleA.ForEach(_ =>
        {
            if (!cycleB.Contains(_))
                result = false;
        });

        return result;
    }

    // Returns true if given point(x,y) is inside the given line segment
    private static bool IsInsideLine(SkeletonLine line, double x, double y)
    {
        return (x >= line.Points.pointA.Position.x && x <= line.Points.pointB.Position.x
                    || x >= line.Points.pointB.Position.x && x <= line.Points.pointA.Position.x)
               && (y >= line.Points.pointA.Position.y && y <= line.Points.pointB.Position.y
                    || y >= line.Points.pointB.Position.y && y <= line.Points.pointA.Position.y);
    }

    //  Returns Point of intersection if do intersect otherwise default Point (null)
    public static Vector2? FindIntersection(this SkeletonLine lineA, SkeletonLine lineB)
        => lineA.ToVector2().FindIntersection(lineB.ToVector2());

    public static IEnumerable<LevelWall> GetLevelWalls(this SkeletonLine skeletonLine)
    {
        var width = 0.15f;

        if (skeletonLine.Type.Name == EntityTypeConstants.Floor.Name || skeletonLine.Type.Name == EntityTypeConstants.Elevator.Name)
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);

            return new List<LevelWall> { WallA, WallB };
        } else if (skeletonLine.Type == EntityTypeConstants.EmptySpaceFloor)
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.y < WallB.Points.pointA.y
                ? WallA
                : WallB;

            return new List<LevelWall> { wall };
        } else if (skeletonLine.Type == EntityTypeConstants.EmptySpaceTop)
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.y > WallB.Points.pointA.y
                ? WallA
                : WallB;

            var airPlatform = WallA.Points.pointA.y <= WallB.Points.pointA.y
                ? WallA
                : WallB;

            airPlatform.Type = EntityTypeConstants.AirPlartform;

            return new List<LevelWall> { wall, airPlatform };
        } else if (skeletonLine.Type == EntityTypeConstants.EmptySpaceElevatorLeft)
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.x < WallB.Points.pointA.x
                ? WallA
                : WallB;

            return new List<LevelWall> { wall };
        } else if (skeletonLine.Type == EntityTypeConstants.EmptySpaceElevatorRight)
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.x > WallB.Points.pointA.x
                ? WallA
                : WallB;

            return new List<LevelWall> { wall };
        } else if (skeletonLine.Type == EntityTypeConstants.InsideFloor)
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.y < WallB.Points.pointA.y
                ? WallA
                : WallB;

            wall.Type = EntityTypeConstants.AirPlartform;
            return new List<LevelWall> { wall };
        }

        return new List<LevelWall>();
    }

    public static LevelWall GetLevelElevator(this SkeletonLine skeletonLine)
    {
        if (skeletonLine.Type.Name.Contains("Elevator"))
        {
            return new LevelWall(skeletonLine.Points.pointA.Position, skeletonLine.Points.pointB.Position, EntityTypeConstants.Elevator);
        }

        return null;
    }

    public static (Vector2 pointA, Vector2 pointB) ToVector2(this SkeletonLine skeletonLine) 
        => (skeletonLine.Points.pointA.Position, skeletonLine.Points.pointB.Position);

    private static (LevelWall WallA, LevelWall WallB) GetWalls(this SkeletonLine skeletonLine, float width)
    {
        var lineAngle = (float)skeletonLine.GetOrigianlLineAngle() * Mathf.Deg2Rad;
        lineAngle += (float)Math.PI / 2;

        var originalAngle = (float)skeletonLine.GetOrigianlLineAngle() * Mathf.Deg2Rad;

        var firstWallX1 = (float)(skeletonLine.Points.pointA.Position.x + width * Math.Cos(lineAngle));
        var firstWallX2 = (float)(skeletonLine.Points.pointB.Position.x + width * Math.Cos(lineAngle));

        firstWallX1 -= 0.1f * (float)Math.Cos(originalAngle + Mathf.PI);
        firstWallX2 -= 0.1f * (float)Math.Cos(originalAngle);

        var firstWallY1 = (float)(skeletonLine.Points.pointA.Position.y + width * Math.Sin(lineAngle));
        var firstWallY2 = (float)(skeletonLine.Points.pointB.Position.y + width * Math.Sin(lineAngle));
        
        firstWallY1 -= 0.1f * (float)Math.Sin(originalAngle + Mathf.PI);
        firstWallY2 -= 0.1f * (float)Math.Sin(originalAngle);

        var secondWallX1 = (float)(skeletonLine.Points.pointA.Position.x - width * Math.Cos(lineAngle));
        var secondWallX2 = (float)(skeletonLine.Points.pointB.Position.x - width * Math.Cos(lineAngle));
        
        secondWallX1 -= 0.1f * (float)Math.Cos(originalAngle + Mathf.PI);
        secondWallX2 -= 0.1f * (float)Math.Cos(originalAngle);

        var secondWallY1 = (float)(skeletonLine.Points.pointA.Position.y - width * Math.Sin(lineAngle));
        var secondWallY2 = (float)(skeletonLine.Points.pointB.Position.y - width * Math.Sin(lineAngle));
        
        secondWallY1 -= 0.1f * (float)Math.Sin(originalAngle + Mathf.PI);
        secondWallY2 -= 0.1f * (float)Math.Sin(originalAngle);

        return (new LevelWall(new Vector2(firstWallX1, firstWallY1), new Vector2(firstWallX2, firstWallY2)), new LevelWall(new Vector2(secondWallX1, secondWallY1), new Vector2(secondWallX2, secondWallY2)));
    }
}