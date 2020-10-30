using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class Vector2Extension
{
    public static float? GetDistanceBetweenLineAndPoint(this (Vector2 pointA, Vector2 pointB) line, Vector2 point)
    {
        var lineAngle = (float)GetOrigianlLineAngle(line);
        var secondLineAngle = (lineAngle + 90) * Mathf.Deg2Rad;

        secondLineAngle = secondLineAngle % Mathf.PI;

        var b = point.y - Mathf.Tan(secondLineAngle) * point.x;

        var x1 = -10000f;
        var x2 = 10000f;
        var y1 = Mathf.Tan(secondLineAngle) * x1 + b;
        var y2 = Mathf.Tan(secondLineAngle) * x2 + b;

        if (Mathf.Abs(Mathf.Abs(secondLineAngle) - Mathf.PI / 2) < 0.01f)
        {
            x1 = point.x;
            x2 = point.x;
            y1 = -10000f;
            y2 = 10000f;
        }

        var intersection = line.FindIntersection((new Vector2(x1, y1), new Vector2(x2, y2)));

        if (intersection == null)
            return null;

        var secondLine = (point, intersection.Value);

        return secondLine.Length();
    }

    // Returns true if given point(x,y) is inside the given line segment
    private static bool IsInsideLine(this (Vector2 pointA, Vector2 pointB) line, double x, double y)
    {
        return (x >= line.pointA.x && x <= line.pointB.x
                    || x >= line.pointB.x && x <= line.pointA.x)
               && (y >= line.pointA.y && y <= line.pointB.y
                    || y >= line.pointB.y && y <= line.pointA.y);
    }

    public static double GetLineAngle(this (Vector2 pointA, Vector2 pointB) line)
    {
        var x1 = line.pointA.x > line.pointB.x ? line.pointB.x : line.pointA.x;
        var x2 = line.pointA.x > line.pointB.x ? line.pointA.x : line.pointB.x;

        var y1 = line.pointA.x > line.pointB.y ? line.pointB.y : line.pointA.y;
        var y2 = line.pointA.x > line.pointB.y ? line.pointA.y : line.pointB.y;

        float xDiff = Mathf.Abs(x2 - x1);
        float yDiff = Mathf.Abs(y2 - y1);
        return Mathf.Atan2(yDiff, xDiff) * 180.0 / Mathf.PI;
    }

    public static double GetOrigianlLineAngle(this (Vector2 pointA, Vector2 pointB) line)
        => Mathf.Atan2(line.pointB.y - line.pointA.y, line.pointB.x - line.pointA.x) * Mathf.Rad2Deg;

    public static Vector2 GetMiddlePoint(this (Vector2 pointA, Vector2 pointB) line)
    {
        var middleX = line.pointA.x > line.pointB.x
            ? line.pointB.x + ((line.pointA.x - line.pointB.x) / 2)
            : line.pointA.x + ((line.pointB.x - line.pointA.x) / 2);

        var middleY = line.pointA.y > line.pointB.y
            ? line.pointB.y + ((line.pointA.y - line.pointB.y) / 2)
            : line.pointA.y + ((line.pointB.y - line.pointA.y) / 2);

        return new Vector2(middleX, middleY);
    }

    public static float Length(this (Vector2 pointA, Vector2 pointB) line) => Vector2.Distance(line.pointA, line.pointB);

    //  Returns Point of intersection if do intersect otherwise default Point (null)
    public static Vector2? FindIntersection(this (Vector2 pointA, Vector2 pointB) lineA, (Vector2 pointA, Vector2 pointB) lineB, bool checkIsInLine = true)
    {
        const double tolerance = 0.001;

        double x1 = lineA.pointA.x, y1 = lineA.pointA.y;
        double x2 = lineA.pointB.x, y2 = lineA.pointB.y;

        double x3 = lineB.pointA.x, y3 = lineB.pointA.y;
        double x4 = lineB.pointB.x, y4 = lineB.pointB.y;

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

        if (Equals(lineA.pointA, lineB.pointA) || Equals(lineA.pointA, lineB.pointB)
            || Equals(lineA.pointB, lineB.pointA) || Equals(lineA.pointA, lineB.pointB))
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
            var m2 = (y4 - y3) / (x4 - x3);
            var c2 = -m2 * x3 + y3;

            //equation of vertical line is x = c
            //if line 1 and 2 intersect then x1=c1=x
            //substitute x=x1 in (4) => -m2x1 + y = c2
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
            var m1 = (y2 - y1) / (x2 - x1);
            var c1 = -m1 * x1 + y1;

            //equation of vertical line is x = c
            //if line 1 and 2 intersect then x3=c3=x
            //substitute x=x3 in (3) => -m1x3 + y = c1
            // => y = c1 + m1x3 
            x = x3;
            y = c1 + m1 * x3;
        }
        //lineA & lineB are not vertical 
        //(could be horizontal we can handle it with slope = 0)
        else
        {
            //compute slope of line 1 (m1) and c2
            var m1 = (y2 - y1) / (x2 - x1);
            var c1 = -m1 * x1 + y1;

            //compute slope of line 2 (m2) and c2
            var m2 = (y4 - y3) / (x4 - x3);
            var c2 = -m2 * x3 + y3;

            //solving equations (3) & (4) => x = (c1-c2)/(m2-m1)
            //plugging x value in equation (4) => y = c2 + m2 * x
            x = (c1 - c2) / (m2 - m1);
            y = c2 + m2 * x;

            //verify by plugging intersection point (x, y)
            //in original equations (1) & (2) to see if they intersect
            //otherwise x,y values will not be finite and will fail this check
            if (!(Math.Abs(-m1 * x + y - c1) < tolerance
                && Math.Abs(-m2 * x + y - c2) < tolerance))
            {
                //return default(Point);
                return null;
            }
        }

        if (!checkIsInLine)
            return new Vector2((float)x, (float)y);

        //x,y can intersect outside the line segment since line is infinitely long
        //so finally check if x, y is within both the line segments
        if (IsInsideLine(lineA, x, y) &&
            IsInsideLine(lineB, x, y))
        {
            return new Vector2((float)x, (float)y);
        }

        //return default null (no intersection)
        return null;
    }

    public static float GetAngle(this Vector2 pointA, Vector2 pointB)
    {
        var diffX = pointB.x - pointA.x;
        var diffY = pointB.y - pointA.y;

        var result = Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg;
        return result;
    }

    public static bool IsPointInside(this List<(Vector2 pointA, Vector2 pointB)> perimeter, Vector2 point)
    {
        var randomAngle = Random.Range(0f, 1.5708f);
        var b = point.y - Mathf.Tan(randomAngle) * point.x;
        var secondPoint = new Vector2(point.x + 1000, Mathf.Tan(randomAngle) * (point.x + 1000) + b);
        var ray = (point, secondPoint);

        var intersectionsCount = perimeter.Count(_ => FindIntersection(_, ray).HasValue);

        return intersectionsCount % 2 != 0;
    }
    
    public static bool ContainsPoint(this (Vector2 pointA, Vector2 pointB) line, Vector2 point) 
        => line.pointA == point || line.pointB == point;

    public static bool IsPointBelongs(this List<(Vector2 pointA, Vector2 pointB)> perimeter, Vector2 point)
        => perimeter.Any(l => l.ContainsPoint(point)) || perimeter.IsPointInside(point);

    public static float AngleBetweenVector2A(this Vector2 vectorA, Vector2 vectorB)
    {
        var difference = vectorB - vectorA;
        var sign = (vectorB.y < vectorA.y) ? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, difference) * sign;
    }

    public static float AngleBetweenVector2(this Vector2 vectorA, Vector2 vectorB)
    {
        var vector3A = new Vector3(vectorA.x, vectorA.y, 0);
        var vector3B = new Vector3(vectorB.x, vectorB.y, 0);
        return Vector3.SignedAngle(vector3A, vector3B, Vector3.forward);
    }

    public static string ToString2(this Vector2 vector)
    {
        return $"{vector.x} {vector.y}";
    }
}