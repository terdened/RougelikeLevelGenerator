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
            var isChanges = true;

            while (isChanges)
            {
                isChanges = false;
                var duplicate = new List<SkeletonLine>();
                foreach(var line in _params.LevelSkeleton.Lines)
                {
                    var duplicates = _params.LevelSkeleton.Lines.Where(l => line != l).Where(l => l.ContainsSkeletonPoint(line.Points.pointA.Position) && l.ContainsSkeletonPoint(line.Points.pointB.Position));

                    if(duplicate.Count > 0)
                    {
                        duplicate.AddRange(duplicates);
                        break;
                    }

                }

                if (duplicate.Count > 0)
                {
                    _params.LevelSkeleton.RemoveLines(duplicate);
                    isChanges = true;
                }
            }

            var skeletonLinesWithWalls = new List<(SkeletonLine line, LevelWall wallA, LevelWall wallB)>();
            foreach (var line in _params.LevelSkeleton.Lines)
            {
                var walls = line.GetLevelWalls().ToArray();

                var wallA = walls.Where(_ => _.Type.Name == "Unknown").Count() > 0 ? walls[0] : null;
                var wallB = walls.Where(_ => _.Type.Name == "Unknown").Count() > 1 ? walls[1] : null;

                skeletonLinesWithWalls.Add((line, wallA, wallB));

                if(walls != null)
                    level.AddWalls(walls);
            }

            foreach (var point in _params.LevelSkeleton.Points.Where(_ => _.Lines.Count >= 2))
            {
                var skeletonLinesWithWallsForPoint = skeletonLinesWithWalls
                    .Where(_ => _.line.ContainsSkeletonPoint(point))
                    .Select(_ =>
                    {
                        if(point == _.line.Points.pointA)
                        {
                            var a = 0;
                        }
                        return (line: new SkeletonLine(point == _.line.Points.pointA ? _.line.Points.pointA : _.line.Points.pointB, point == _.line.Points.pointA ? _.line.Points.pointB : _.line.Points.pointA), _.wallA, _.wallB);
                    }).ToArray();

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

                skeletonLinesWithWallsForPoint = skeletonLinesWithWallsForPoint.OrderBy(_ => GetAngle(point.Position, _.line.Points.pointB.Position)).ToArray();

                if (skeletonLinesWithWallsForPoint.Count() < 2)
                    continue;

                for(int i = 0; i < skeletonLinesWithWallsForPoint.Count(); i++)
                {
                    var angle = GetAngle(point.Position, skeletonLinesWithWallsForPoint[i].line.Points.pointB.Position);
                    var wallAAngle = skeletonLinesWithWallsForPoint[i].wallA != null ? GetAngle(point.Position, skeletonLinesWithWallsForPoint[i].wallA.Points.pointB) : (float?)null;

                    if (wallAAngle.HasValue)
                    {
                        if (Mathf.Abs(wallAAngle.Value - angle) > 180)
                        {
                            if (wallAAngle > angle)
                                wallAAngle -= 360;
                            else
                                angle -= 360;
                        }

                        if (angle > wallAAngle)
                        {
                            var wall = skeletonLinesWithWallsForPoint[i].wallB;
                            skeletonLinesWithWallsForPoint[i].wallB = skeletonLinesWithWallsForPoint[i].wallA;
                            skeletonLinesWithWallsForPoint[i].wallA = wall;
                        }
                    } else
                    {
                        var wallBAngle = skeletonLinesWithWallsForPoint[i].wallA != null ? GetAngle(point.Position, skeletonLinesWithWallsForPoint[i].wallB.Points.pointB) : (float?)null;
                        if (wallBAngle.HasValue)
                        {
                            if (Mathf.Abs(wallBAngle.Value - angle) > 180)
                            {
                                if (wallBAngle > angle)
                                    wallBAngle -= 360;
                                else
                                    angle -= 360;
                            }

                            if (angle < wallBAngle)
                            {
                                var wall = skeletonLinesWithWallsForPoint[i].wallB;
                                skeletonLinesWithWallsForPoint[i].wallB = skeletonLinesWithWallsForPoint[i].wallA;
                                skeletonLinesWithWallsForPoint[i].wallA = wall;
                            }
                        }
                    }

                }

                //skeletonLinesWithWallsForPoint.ForEach(_ =>
                //{
                //    var angle = GetAngle(point.Position, _.line.Points.pointB.Position);
                //    var wallAAngle = GetAngle(point.Position, _.wallA.Points.pointB);

                //    if (Mathf.Abs(wallAAngle - angle) > 180)
                //    {
                //        if (wallAAngle > angle)
                //            wallAAngle -= 360;
                //        else
                //            angle -= 360;
                //    }

                //    if (angle > wallAAngle)
                //    {
                //        var wall = _.wallB;
                //        _.wallB = _.wallA;
                //        _.wallA = wall;
                //    }
                //});

                for (var i = 0; i < skeletonLinesWithWallsForPoint.Count(); i++)
                {
                    var wallA = skeletonLinesWithWallsForPoint[i].wallA;
                    var wallB = i < skeletonLinesWithWallsForPoint.Count() - 1
                                    ? skeletonLinesWithWallsForPoint[i + 1].wallB
                                    : skeletonLinesWithWallsForPoint[0].wallB;
                    if (wallA != null && wallB != null)
                    {
                        var intersection = wallA.FindIntersection(wallB);

                        if (intersection.HasValue)
                        {
                            wallA.SetPointA(intersection.Value);
                            wallB.SetPointA(intersection.Value);
                        }
                        //else if (Mathf.Abs(wallA.Points.pointA.x - wallB.Points.pointA.x) < 0.01 || Mathf.Abs(wallA.Points.pointA.y - wallB.Points.pointA.y) < 0.01)
                        //{
                        //    var middlePoint = new Vector2((wallA.Points.pointA.x + wallB.Points.pointA.x) / 2, (wallA.Points.pointA.y + wallB.Points.pointA.y) / 2);
                        //    wallA.SetPointA(middlePoint);
                        //    wallB.SetPointA(middlePoint);
                        //}
                    }
                }
            }
            /////////////////////////////

            //foreach (var wall in level.Walls.Where(_ => _.Type.Name == "Unknown"))
            //{
            //    var otherWalls = level.Walls.Where(_ => _.Type.Name == "Unknown").Where(_ => _.Id != wall.Id).ToList();

            //    var allIntersectionsForPointA = otherWalls.Select(_ =>
            //    {
            //        var intersection = _.FindIntersection(wall);
            //        return (Intersection: intersection, Distance: intersection.HasValue ? Vector2.Distance(intersection.Value, wall.Points.pointA) : (float?)null);
            //    });

            //    var intersectionForPointA = allIntersectionsForPointA.Where(_ => _.Distance.HasValue && _.Distance.Value > 0.001).OrderBy(_ => _.Distance).First();

            //    if (intersectionForPointA.Distance < 1f)
            //        wall.SetPointA(intersectionForPointA.Intersection.Value);
            //    else
            //    {
            //        var a = allIntersectionsForPointA.Where(_ => _.Distance.HasValue).Min(_ => _.Distance);
            //        a = 0;
            //    }

            //var allIntersectionsForPointB = otherWalls.Select(_ =>
            //{
            //    var intersection = _.FindIntersection(wall);
            //    return (Intersection: intersection, Distance: intersection.HasValue ? Vector2.Distance(intersection.Value, wall.Points.pointB) : (float?)null);
            //});

            //var firstIntersectionForPointB = allIntersectionsForPointB.Where(_ => _.Distance.HasValue).OrderBy(_ => _.Distance).First().Intersection;

            //var intersectionForPointB = allIntersectionsForPointB.Where(_ => _.Distance.HasValue).OrderBy(_ => _.Distance).First();

            //if (intersectionForPointB.Distance < 1f)
            //    wall.SetPointB(intersectionForPointB.Intersection.Value);
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

    private float GetAngle(Vector2 vectorA, Vector2 vectorB)
    {
        var diffX = vectorB.x - vectorA.x;
        var diffY = vectorB.y - vectorA.y;

        var result = Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg;
        return result;
    }
}
