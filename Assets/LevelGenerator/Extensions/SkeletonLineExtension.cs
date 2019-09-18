using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class SkeletonLineExtension
{
    public static float? GetDistanceBetweenLineAndPoint(this SkeletonLine line, SkeletonPoint point)
    {
        var lineAngle = (float)GetOrigianlLineAngle(line);
        var secondLineAngle = (lineAngle + 90) * Mathf.Deg2Rad;

        secondLineAngle = secondLineAngle % Mathf.PI;

        //if (Mathf.Abs(Mathf.Abs(secondLineAngle) - Mathf.PI / 2) < 0.01f)
        //    return null;

        var b = point.Position.y - Mathf.Tan(secondLineAngle) * point.Position.x;

        var x1 = -10000;
        var x2 = 10000;
        var y1 = Mathf.Tan(secondLineAngle) * x1 + b;
        var y2 = Mathf.Tan(secondLineAngle) * x2 + b;

        var intersection = line.FindIntersection(new SkeletonLine(new SkeletonPoint(new Vector2(x1, y1)), new SkeletonPoint(new Vector2(x2, y2))));

        if (intersection == null)
            return null;

        var secondLine = new SkeletonLine(point, new SkeletonPoint(intersection.Value));

        if(secondLine.Length < 0.5)
        {
            var a = 0;
        }

        return secondLine.Length;
    }

    public static double GetLineAngle(this SkeletonLine line)
    {
        var x1 = line.Points.pointA.Position.x > line.Points.pointB.Position.x ? line.Points.pointB.Position.x : line.Points.pointA.Position.x;
        var x2 = line.Points.pointA.Position.x > line.Points.pointB.Position.x ? line.Points.pointA.Position.x : line.Points.pointB.Position.x;

        var y1 = line.Points.pointA.Position.x > line.Points.pointB.Position.y ? line.Points.pointB.Position.y : line.Points.pointA.Position.y;
        var y2 = line.Points.pointA.Position.x > line.Points.pointB.Position.y ? line.Points.pointA.Position.y : line.Points.pointB.Position.y;

        float xDiff = Mathf.Abs(x2 - x1);
        float yDiff = Mathf.Abs(y2 - y1);
        return Mathf.Atan2(yDiff, xDiff) * 180.0 / Mathf.PI;
    }

    public static double GetOrigianlLineAngle(this SkeletonLine line)
    {
        return Mathf.Atan2(line.Points.pointB.Position.y - line.Points.pointA.Position.y, line.Points.pointB.Position.x - line.Points.pointA.Position.x) * 180.0 / Mathf.PI;
    }

    public static SkeletonPoint GetMiddlePoint(this SkeletonLine line)
    {
        var middleX = line.Points.pointA.Position.x > line.Points.pointB.Position.x
            ? line.Points.pointB.Position.x + ((line.Points.pointA.Position.x - line.Points.pointB.Position.x) / 2)
            : line.Points.pointA.Position.x + ((line.Points.pointB.Position.x - line.Points.pointA.Position.x) / 2);
        
        var middleY = line.Points.pointA.Position.y > line.Points.pointB.Position.y
            ? line.Points.pointB.Position.y + ((line.Points.pointA.Position.y - line.Points.pointB.Position.y) / 2)
            : line.Points.pointA.Position.y + ((line.Points.pointB.Position.y - line.Points.pointA.Position.y) / 2);

        return new SkeletonPoint(new Vector2(middleX, middleY));
    }

    public static bool IsSkeletonPointInside(this List<SkeletonLine> perimeter, SkeletonPoint point)
    {
        var randomAngle = Random.Range(0f, 1.5708f);
        var b = point.Position.y - Mathf.Tan(randomAngle) * point.Position.x;
        var secondPoint = new SkeletonPoint(new Vector2(1000, Mathf.Tan(randomAngle) * 1000 + b));
        var ray = new SkeletonLine(point, secondPoint);

        while (perimeter.Any(_ => (ray.GetDistanceBetweenLineAndPoint(_.Points.pointA) == -1 ? 0 : ray.GetDistanceBetweenLineAndPoint(_.Points.pointA)) < 0.1 || (ray.GetDistanceBetweenLineAndPoint(_.Points.pointB) == -1 ? 0: ray.GetDistanceBetweenLineAndPoint(_.Points.pointB)) < 0.1))
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
    {
        double tolerance = 0.001;

        double x1 = lineA.Points.pointA.Position.x, y1 = lineA.Points.pointA.Position.y;
        double x2 = lineA.Points.pointB.Position.x, y2 = lineA.Points.pointB.Position.y;

        double x3 = lineB.Points.pointA.Position.x, y3 = lineB.Points.pointA.Position.y;
        double x4 = lineB.Points.pointB.Position.x, y4 = lineB.Points.pointB.Position.y;

        // equations of the form x = c (two vertical lines)
        if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance && Math.Abs(x1 - x3) < tolerance)
        {
            return null;
        }

        //equations of the form y=c (two horizontal lines)
        if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance && Math.Abs(y1 - y3) < tolerance)
        {
            return null;
        }

        //equations of the form x=c (two vertical lines)
        if (Math.Abs(x1 - x2) < tolerance && Math.Abs(x3 - x4) < tolerance)
        {
            // return default(Point);
            return null;
        }

        //equations of the form y=c (two horizontal lines)
        if (Math.Abs(y1 - y2) < tolerance && Math.Abs(y3 - y4) < tolerance)
        {
            // return default(Point);
            return null;
        }

        if (lineA.Points.pointA == lineB.Points.pointA
            || lineA.Points.pointA == lineB.Points.pointB
            || lineA.Points.pointB == lineB.Points.pointA
            || lineA.Points.pointA == lineB.Points.pointB)
            return null;

        //general equation of line is y = mx + c where m is the slope
        //assume equation of line 1 as y1 = m1x1 + c1 
        //=> -m1x1 + y1 = c1 ----(1)
        //assume equation of line 2 as y2 = m2x2 + c2
        //=> -m2x2 + y2 = c2 -----(2)
        //if line 1 and 2 intersect then x1=x2=x & y1=y2=y where (x,y) is the intersection point
        //so we will get below two equations 
        //-m1x + y = c1 --------(3)
        //-m2x + y = c2 --------(4)

        double x, y;

        //lineA is vertical x1 = x2
        //slope will be infinity
        //so lets derive another solution
        if (Math.Abs(x1 - x2) < tolerance)
        {
            //compute slope of line 2 (m2) and c2
            double m2 = (y4 - y3) / (x4 - x3);
            double c2 = -m2 * x3 + y3;

            //equation of vertical line is x = c
            //if line 1 and 2 intersect then x1=c1=x
            //subsitute x=x1 in (4) => -m2x1 + y = c2
            // => y = c2 + m2x1 
            x = x1;
            y = c2 + m2 * x1;
        }
        //lineB is vertical x3 = x4
        //slope will be infinity
        //so lets derive another solution
        else if (Math.Abs(x3 - x4) < tolerance)
        {
            //compute slope of line 1 (m1) and c2
            double m1 = (y2 - y1) / (x2 - x1);
            double c1 = -m1 * x1 + y1;

            //equation of vertical line is x = c
            //if line 1 and 2 intersect then x3=c3=x
            //subsitute x=x3 in (3) => -m1x3 + y = c1
            // => y = c1 + m1x3 
            x = x3;
            y = c1 + m1 * x3;
        }
        //lineA & lineB are not vertical 
        //(could be horizontal we can handle it with slope = 0)
        else
        {
            //compute slope of line 1 (m1) and c2
            double m1 = (y2 - y1) / (x2 - x1);
            double c1 = -m1 * x1 + y1;

            //compute slope of line 2 (m2) and c2
            double m2 = (y4 - y3) / (x4 - x3);
            double c2 = -m2 * x3 + y3;

            //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
            //plugging x value in equation (4) => y = c2 + m2 * x
            x = (c1 - c2) / (m2 - m1);
            y = c2 + m2 * x;

            //verify by plugging intersection point (x, y)
            //in orginal equations (1) & (2) to see if they intersect
            //otherwise x,y values will not be finite and will fail this check
            if (!(Math.Abs(-m1 * x + y - c1) < tolerance
                && Math.Abs(-m2 * x + y - c2) < tolerance))
            {
                //return default(Point);
                return null;
            }
        }

        //x,y can intersect outside the line segment since line is infinitely long
        //so finally check if x, y is within both the line segments
        if (IsInsideLine(lineA, x, y) &&
            IsInsideLine(lineB, x, y))
        {

            //Debug.Log($"Line A ({lineA.Points.pointA.Position.x}, {lineA.Points.pointA.Position.y}) ({lineA.Points.pointB.Position.x}, {lineA.Points.pointB.Position.y})");
            //Debug.Log($"Line B ({lineB.Points.pointA.Position.x}, {lineB.Points.pointA.Position.y}) ({lineB.Points.pointB.Position.x}, {lineB.Points.pointB.Position.y})");
            //Debug.Log($"Intersection ({x}, {y})");
            return new Vector2((float) x, (float) y);
            // return new Point { x = x, y = y };
        }

        //return default null (no intersection)
        // return default(Point);
        return null;

    }

    public static IEnumerable<LevelWall> GetLevelWalls(this SkeletonLine skeletonLine)
    {
        var width = 0.15f;

        if (skeletonLine.Type.Name == "Floor" || skeletonLine.Type.Name == "Elevator")
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);

            return new List<LevelWall> { WallA, WallB };
        } else if (skeletonLine.Type.Name == "Empty Space Floor")
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.y < WallB.Points.pointA.y
                ? WallA
                : WallB;

            return new List<LevelWall> { wall };
        } else if (skeletonLine.Type.Name == "Empty Space Roof")
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.y > WallB.Points.pointA.y
                ? WallA
                : WallB;

            var airPlatform = WallA.Points.pointA.y <= WallB.Points.pointA.y
                ? WallA
                : WallB;

            airPlatform.Type = new EntityType(Color.yellow, "Air Plartform");

            return new List<LevelWall> { wall, airPlatform };
        } else if (skeletonLine.Type.Name == "Empty Space Elevator Left")
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.x < WallB.Points.pointA.x
                ? WallA
                : WallB;

            return new List<LevelWall> { wall };
        } else if (skeletonLine.Type.Name == "Empty Space Elevator Right")
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.x > WallB.Points.pointA.x
                ? WallA
                : WallB;

            return new List<LevelWall> { wall };
        } else if (skeletonLine.Type.Name == "Inside Floor")
        {
            var (WallA, WallB) = GetWalls(skeletonLine, width);
            var wall = WallA.Points.pointA.y < WallB.Points.pointA.y
                ? WallA
                : WallB;

            wall.Type = new EntityType(Color.yellow, "Air Plartform");
            return new List<LevelWall> { wall };
        }

        return new List<LevelWall>();
    }

    public static LevelWall GetLevelElevator(this SkeletonLine skeletonLine)
    {
        if (skeletonLine.Type.Name.Contains("Elevator"))
        {
            return new LevelWall(skeletonLine.Points.pointA.Position, skeletonLine.Points.pointB.Position, new EntityType(Color.green, "Elevator"));
        }

        return null;
    }

    private static (LevelWall WallA, LevelWall WallB) GetWalls(this SkeletonLine skeletonLine, float width)
    {
        var lineAngle = (float)skeletonLine.GetOrigianlLineAngle() * Mathf.Deg2Rad;
        lineAngle += (float)Math.PI / 2;

        var originalAngle = (float)skeletonLine.GetOrigianlLineAngle() * Mathf.Deg2Rad;

        var firstWallX1 = (float)(skeletonLine.Points.pointA.Position.x + width * Math.Cos(lineAngle));
        var firstWallX2 = (float)(skeletonLine.Points.pointB.Position.x + width * Math.Cos(lineAngle));

        firstWallX1 -= 0.3f * (float)Math.Cos(originalAngle + Mathf.PI);
        firstWallX2 -= 0.3f * (float)Math.Cos(originalAngle);

        var firstWallY1 = (float)(skeletonLine.Points.pointA.Position.y + width * Math.Sin(lineAngle));
        var firstWallY2 = (float)(skeletonLine.Points.pointB.Position.y + width * Math.Sin(lineAngle));
        
        firstWallY1 -= 0.3f * (float)Math.Sin(originalAngle + Mathf.PI);
        firstWallY2 -= 0.3f * (float)Math.Sin(originalAngle);

        var secondWallX1 = (float)(skeletonLine.Points.pointA.Position.x - width * Math.Cos(lineAngle));
        var secondWallX2 = (float)(skeletonLine.Points.pointB.Position.x - width * Math.Cos(lineAngle));
        
        secondWallX1 -= 0.3f * (float)Math.Cos(originalAngle + Mathf.PI);
        secondWallX2 -= 0.3f * (float)Math.Cos(originalAngle);

        var secondWallY1 = (float)(skeletonLine.Points.pointA.Position.y - width * Math.Sin(lineAngle));
        var secondWallY2 = (float)(skeletonLine.Points.pointB.Position.y - width * Math.Sin(lineAngle));
        
        secondWallY1 -= 0.3f * (float)Math.Sin(originalAngle + Mathf.PI);
        secondWallY2 -= 0.3f * (float)Math.Sin(originalAngle);

        return (new LevelWall(new Vector2(firstWallX1, firstWallY1), new Vector2(firstWallX2, firstWallY2)), new LevelWall(new Vector2(secondWallX1, secondWallY1), new Vector2(secondWallX2, secondWallY2)));
    }
}