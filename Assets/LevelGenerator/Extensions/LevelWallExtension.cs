using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelWallExtension
{
    //  Returns Point of intersection if do intersect otherwise default Point (null)
    public static Vector2? FindIntersection(this LevelWall wallA, LevelWall wallB)
    {
        double tolerance = 0.001;

        double x1 = wallA.Points.pointA.x, y1 = wallA.Points.pointA.y;
        double x2 = wallA.Points.pointB.x, y2 = wallA.Points.pointB.y;

        double x3 = wallB.Points.pointA.x, y3 = wallB.Points.pointA.y;
        double x4 = wallB.Points.pointB.x, y4 = wallB.Points.pointB.y;

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

        if (wallA.Points.pointA == wallB.Points.pointA
            || wallA.Points.pointA == wallB.Points.pointB
            || wallA.Points.pointB == wallB.Points.pointA
            || wallA.Points.pointA == wallB.Points.pointB)
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
        
        return new Vector2((float)x, (float)y);
    }
}