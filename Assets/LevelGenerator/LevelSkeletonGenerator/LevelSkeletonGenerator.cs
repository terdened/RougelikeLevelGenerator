using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSkeletonGenerator : BaseGenerator<LevelSkeleton, LevelSkeletonGeneratorParams>
{
    private readonly SkeletonPointGenerator _skeletonPointGenerator;

    public LevelSkeletonGenerator(LevelSkeletonGeneratorParams generatorParams, IEnumerable<IGeneratorCriteria<LevelSkeleton>> generatorCriteria = null) : base(generatorParams, generatorCriteria)
    {
        var criterias = new List<IGeneratorCriteria<List<SkeletonPoint>>>
        {
            new AvoidNearSkeletonPointGeneratorCriteria()
        };

        _skeletonPointGenerator = new SkeletonPointGenerator(generatorParams.SkeletonPointGeneratorParams, criterias);
    }

    protected override LevelSkeleton Generate()
    {
        var randomPoints = new LevelSkeleton();


        //1. Generate random points
        var points = _skeletonPointGenerator.Execute();
        randomPoints.AddPoints(points);

        //2. Generate full connected graph
        var fullSkeletonLines = randomPoints.ToGraph().ToFullGraph().Edges.ToList().Select(_ =>
        {
            return new SkeletonLine(_.Vertexes.ToList()[0].Data, _.Vertexes.ToList()[1].Data);
        });

        var fullSkeleton = new LevelSkeleton();
        fullSkeleton.AddLines(fullSkeletonLines);


        //3. Get basement
        var basementSkeleton = fullSkeleton.ToBasementSkeleton();

        //4. Add additional lines

        var triangleSkeleton = fullSkeleton.ToTriangleSkeleton();
        var notUsedLines = triangleSkeleton.Lines.Where(f => !basementSkeleton.Lines.Any(r => f.Points.pointA == r.Points.pointA && f.Points.pointB == r.Points.pointB
                                                                                || f.Points.pointA == r.Points.pointB && f.Points.pointB == r.Points.pointA))
                                                                                .ToList();

        var result = new LevelSkeleton();
        result.AddLines(basementSkeleton.Lines);

        var additionalLines = new List<SkeletonLine>();
        var i = 0;

        while (i < _params.AdditionalLinesCount)
        {
            var randomLine = notUsedLines[Random.Range(0, notUsedLines.Count - 1)];

            if (result.Lines.Any(_ => _.FindIntersection(randomLine).HasValue))
                continue;

            result.AddLine(randomLine);
            notUsedLines.Remove(randomLine);
            i++;
        }

        //5. Replace all edges with angle more then 45 deegree

        var linesToRemove = new List<SkeletonLine>();

        if(_params.ReplaceMoreThan45)
        {
            result.Lines.ToList().ForEach(_ =>
            {
                if (GetLineAngle(_) > 45)
                {
                    var point = new SkeletonPoint(new Vector2(_.Points.pointA.Position.x, _.Points.pointB.Position.y), new EntityType(Color.blue, "Additional"));
                    var line1 = new SkeletonLine(_.Points.pointA, point, new EntityType(Color.green, "Corrected"));
                    var line2 = new SkeletonLine(_.Points.pointB, point, new EntityType(Color.green, "Corrected"));

                    result.AddLine(line1);
                    result.AddLine(line2);
                    linesToRemove.Add(_);
                }
            });

            result.RemoveLines(linesToRemove);
        }
        

        //6. Merge points near with line

        if(_params.MergeNearBy)
        {
            var isStart = true;

            while (linesToRemove.Count > 0 || isStart)
            {
                isStart = false;
                linesToRemove = new List<SkeletonLine>();

                foreach (var point in result.Points)
                {
                    var lineToRemove = result.Lines.Where(l => l.Points.pointA != point && l.Points.pointB != point && GetDistanceBetweenLineAndPoint(l, point) < 0.5)
                        .FirstOrDefault();

                    if (lineToRemove != null)
                    {
                        linesToRemove.Add(lineToRemove);

                        var line1 = new SkeletonLine(point, lineToRemove.Points.pointA);
                        var line2 = new SkeletonLine(point, lineToRemove.Points.pointB);

                        result.AddLine(line1);
                        result.AddLine(line2);
                        point.Type = new EntityType(Color.red, "NearByLine");

                        break;
                    }
                }

                result.RemoveLines(linesToRemove);
            }
        }
        

        return result;
    }

    public double GetLineAngle(SkeletonLine line)
    {
        var x1 = line.Points.pointA.Position.x > line.Points.pointB.Position.x ? line.Points.pointB.Position.x : line.Points.pointA.Position.x;
        var x2 = line.Points.pointA.Position.x > line.Points.pointB.Position.x ? line.Points.pointA.Position.x : line.Points.pointB.Position.x;
        
        var y1 = line.Points.pointA.Position.y > line.Points.pointB.Position.y ? line.Points.pointB.Position.y : line.Points.pointA.Position.y;
        var y2 = line.Points.pointA.Position.y > line.Points.pointB.Position.y ? line.Points.pointA.Position.y : line.Points.pointB.Position.y;

        float xDiff = Mathf.Abs(x2 - x1);
        float yDiff = Mathf.Abs(y2 - y1);
        return Mathf.Atan2(yDiff, xDiff) * 180.0 / Mathf.PI;
    }

    public float? GetDistanceBetweenLineAndPoint(SkeletonLine line, SkeletonPoint point)
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
}
