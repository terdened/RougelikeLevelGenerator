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

    public static double GetOriginalLineAngle(this SkeletonLine line)
        => line.ToVector2().GetOrigianlLineAngle();

    public static SkeletonPoint GetMiddlePoint(this SkeletonLine line)
        => new SkeletonPoint(line.ToVector2().GetMiddlePoint());

    public static bool IsSkeletonPointInsideStatistics(this List<SkeletonLine> perimeter, SkeletonPoint point, int repeats = 9)
    {
        var yesCount = 0;
        var noCounts = 0;

        var attempts = 0;
        while (attempts < repeats)
        {
            attempts++;

            if(IsSkeletonPointInside(perimeter, point))
            {
                yesCount++;
            } else
            {
                noCounts++;
            }
        }

        return yesCount >= noCounts;
    }

    public static bool IsSkeletonPointInside(this List<SkeletonLine> perimeter, SkeletonPoint point)
    {
        var randomAngle = Random.Range(0f, 1.5708f);
        var b = point.Position.y - Mathf.Tan(randomAngle) * point.Position.x;
        var secondPoint = new SkeletonPoint(new Vector2(point.Position.x + 100, Mathf.Tan(randomAngle) * (point.Position.x + 100) + b));
        var ray = new SkeletonLine(point, secondPoint);
        
        var intersectionsCount = perimeter.Count(_ => FindIntersection(_, ray).HasValue);

        return intersectionsCount % 2 != 0;
    }

    public static bool IsSkeletonPointBelongs(this List<SkeletonLine> perimeter, SkeletonPoint point)
      => perimeter.Any(l => l.ContainsSkeletonPoint(point)) || perimeter.IsSkeletonPointInside(point);
    
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
    
    //  Returns Point of intersection if do intersect otherwise default Point (null)
    public static Vector2? FindIntersection(this SkeletonLine lineA, SkeletonLine lineB)
        => lineA.ToVector2().FindIntersection(lineB.ToVector2());

    public static IEnumerable<LevelWall> GetLevelWalls(this SkeletonLine skeletonLine)
    {
        const float width = 0.15f;

        if (skeletonLine.Type.Name == EntityTypeConstants.Floor.Name || skeletonLine.Type.Name == EntityTypeConstants.Elevator.Name)
        {
            var (wallA, wallB) = GetWalls(skeletonLine, width);

            return new List<LevelWall> { wallA, wallB };
        }

        if (skeletonLine.Type == EntityTypeConstants.EmptySpaceFloor)
        {
            var (wallA, wallB) = GetWalls(skeletonLine, width);
            var wall = wallA.Points.pointA.y < wallB.Points.pointA.y
                ? wallA
                : wallB;

            return new List<LevelWall> { wall };
        }

        if (skeletonLine.Type == EntityTypeConstants.EmptySpaceTop)
        {
            var (wallA, wallB) = GetWalls(skeletonLine, width);
            var wall = wallA.Points.pointA.y > wallB.Points.pointA.y
                ? wallA
                : wallB;

            var airPlatform = wallA.Points.pointA.y <= wallB.Points.pointA.y
                ? wallA
                : wallB;

            airPlatform.Type = EntityTypeConstants.AirPlatform;

            return new List<LevelWall> { wall, airPlatform };
        }

        if (skeletonLine.Type == EntityTypeConstants.EmptySpaceElevatorLeft)
        {
            var (wallA, wallB) = GetWalls(skeletonLine, width);
            var wall = wallA.Points.pointA.x < wallB.Points.pointA.x
                ? wallA
                : wallB;

            return new List<LevelWall> { wall };
        }

        if (skeletonLine.Type == EntityTypeConstants.EmptySpaceElevatorRight)
        {
            var (wallA, wallB) = GetWalls(skeletonLine, width);
            var wall = wallA.Points.pointA.x > wallB.Points.pointA.x
                ? wallA
                : wallB;

            return new List<LevelWall> { wall };
        }

        if (skeletonLine.Type == EntityTypeConstants.InsideFloor)
        {
            var (wallA, wallB) = GetWalls(skeletonLine, width);
            var wall = wallA.Points.pointA.y < wallB.Points.pointA.y
                ? wallA
                : wallB;

            wall.Type = EntityTypeConstants.AirPlatform;
            return new List<LevelWall> { wall };
        }

        return new List<LevelWall>();
    }

    public static LevelElevator GetLevelElevator(this SkeletonLine skeletonLine)
     => skeletonLine.Type.Name.Contains("Elevator") 
         ? new LevelElevator(skeletonLine.Points.pointA.Position, skeletonLine.Points.pointB.Position, EntityTypeConstants.Elevator) 
         : null;
    
    public static (Vector2 pointA, Vector2 pointB) ToVector2(this SkeletonLine skeletonLine) 
        => (skeletonLine.Points.pointA.Position, skeletonLine.Points.pointB.Position);

    private static (LevelWall WallA, LevelWall WallB) GetWalls(this SkeletonLine skeletonLine, float width)
    {
        var lineAngle = (float)skeletonLine.GetOriginalLineAngle() * Mathf.Deg2Rad;
        lineAngle += (float)Math.PI / 2;

        var originalAngle = (float)skeletonLine.GetOriginalLineAngle() * Mathf.Deg2Rad;

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