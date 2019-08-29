using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SkeletonLineExtension
{
    public static float? GetDistanceBetweenLineAndPoint(this SkeletonLine line, SkeletonPoint point)
    {
        var lineAngle = (float)GetLineAngle(line);
        var secondLineAngle = lineAngle + 90;
        var b = point.Position.y - Mathf.Atan(secondLineAngle) * point.Position.x;

        var x1 = -10000;
        var x2 = 10000;
        var y1 = Mathf.Atan(secondLineAngle) * x1 + b;
        var y2 = Mathf.Atan(secondLineAngle) * x2 + b;

        var intersection = line.FindIntersection(new SkeletonLine(new SkeletonPoint(new Vector2(x1, y1)), new SkeletonPoint(new Vector2(x2, y2))));

        if (intersection == null)
            return null;

        var secondLine = new SkeletonLine(point, new SkeletonPoint(intersection.Value));

        return secondLine.Length;
    }

    public static double GetLineAngle(this SkeletonLine line)
    {
        var x1 = line.Points.pointA.Position.x > line.Points.pointB.Position.x ? line.Points.pointB.Position.x : line.Points.pointA.Position.x;
        var x2 = line.Points.pointA.Position.x > line.Points.pointB.Position.x ? line.Points.pointA.Position.x : line.Points.pointB.Position.x;

        var y1 = line.Points.pointA.Position.y > line.Points.pointB.Position.y ? line.Points.pointB.Position.y : line.Points.pointA.Position.y;
        var y2 = line.Points.pointA.Position.y > line.Points.pointB.Position.y ? line.Points.pointA.Position.y : line.Points.pointB.Position.y;

        float xDiff = Mathf.Abs(x2 - x1);
        float yDiff = Mathf.Abs(y2 - y1);
        return Mathf.Atan2(yDiff, xDiff) * 180.0 / Mathf.PI;
    }

    public static double GetPathLength(this List<SkeletonLine> path) => path.Sum(_ => _.Length);

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
}