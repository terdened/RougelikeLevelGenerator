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
        var generatedPoints = new LevelSkeleton();
        var result = new LevelSkeleton();

        //1. Generate random points
        var points = _skeletonPointGenerator.Execute();
        generatedPoints.AddPoints(points);

        //2. Generate full connected graph
        var fullSkeleton = new LevelSkeleton();
        var fullSkeletonLines = generatedPoints.ToGraph().ToFullGraph().Edges.ToList()
                                               .Select(_ => new SkeletonLine(_.Vertexes.ToList()[0].Data, _.Vertexes.ToList()[1].Data));
        fullSkeleton.AddLines(fullSkeletonLines);

        //3. Get basement
        var basementSkeleton = fullSkeleton.ToBasementSkeleton();
        result.AddLines(basementSkeleton.Lines);

        //4. Add additional lines
        if (_params.AdditionalLinesCount > 0)
            AddAdditionalLines(result, fullSkeleton, basementSkeleton);

        //5. Replace all edges with angle more then 45 deegree
        if (_params.ReplaceMoreThan45)
            ReplaceMoreThan45(result);

        //6. Merge points near with line
        if (_params.MergeNearBy)
            MergeNearBy(result);

        var cycles = result.GetCycles();
        cycles.ForEach(cycle => cycle.ForEach(line => line.Type = new EntityType(Color.magenta, "Cycle")));
        Debug.Log(cycles.Count);

        return result;
    }

    private void AddAdditionalLines(LevelSkeleton skeleton, LevelSkeleton fullSkeleton, LevelSkeleton basementSkeleton)
    {
        var triangleSkeleton = fullSkeleton.ToTriangleSkeleton();
        var notUsedLines = triangleSkeleton.Lines.Where(f => !basementSkeleton.Lines.Any(r => f.ContainsSkeletonPoint(r.Points.pointA)
                                                                                              && f.ContainsSkeletonPoint(r.Points.pointB))
                                                        ).ToList();


        var additionalLines = new List<SkeletonLine>();
        var i = 0;

        while (i < _params.AdditionalLinesCount)
        {
            var randomLine = notUsedLines[Random.Range(0, notUsedLines.Count - 1)];

            if (skeleton.Lines.Any(_ => _.FindIntersection(randomLine).HasValue))
                continue;

            skeleton.AddLine(randomLine);
            notUsedLines.Remove(randomLine);
            i++;
        }
    }

    private void ReplaceMoreThan45(LevelSkeleton skeleton)
    {
        var linesToRemove = new List<SkeletonLine>();
        skeleton.Lines.ToList().ForEach(_ =>
        {
            if (_.GetLineAngle() > 45)
            {
                var point = new SkeletonPoint(new Vector2(_.Points.pointA.Position.x, _.Points.pointB.Position.y), new EntityType(Color.blue, "Additional"));
                var line1 = new SkeletonLine(_.Points.pointA, point, new EntityType(Color.green, "Corrected"));
                var line2 = new SkeletonLine(_.Points.pointB, point, new EntityType(Color.green, "Corrected"));

                skeleton.AddLine(line1);
                skeleton.AddLine(line2);
                linesToRemove.Add(_);
            }
        });

        skeleton.RemoveLines(linesToRemove);
    }

    private void MergeNearBy(LevelSkeleton skeleton)
    {
        var linesToRemove = new List<SkeletonLine>();
        var isStart = true;

        while (linesToRemove.Count > 0 || isStart)
        {
            isStart = false;
            linesToRemove = new List<SkeletonLine>();

            foreach (var point in skeleton.Points)
            {
                var lineToRemove = skeleton.Lines.Where(l => l.Points.pointA != point && l.Points.pointB != point && l.GetDistanceBetweenLineAndPoint(point) < 0.5)
                    .FirstOrDefault();

                if (lineToRemove != null)
                {
                    linesToRemove.Add(lineToRemove);

                    var line1 = new SkeletonLine(point, lineToRemove.Points.pointA);
                    var line2 = new SkeletonLine(point, lineToRemove.Points.pointB);

                    skeleton.AddLine(line1);
                    skeleton.AddLine(line2);
                    point.Type = new EntityType(Color.red, "NearByLine");

                    break;
                }
            }

            skeleton.RemoveLines(linesToRemove);
        }
    }
}
