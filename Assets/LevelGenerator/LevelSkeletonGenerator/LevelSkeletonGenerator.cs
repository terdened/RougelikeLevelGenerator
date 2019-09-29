using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSkeletonGenerator : BaseGenerator<LevelSkeleton, LevelSkeletonGeneratorParams>
{
    private readonly SkeletonPointsGenerator _skeletonPointsGenerator;

    public LevelSkeletonGenerator(LevelSkeletonGeneratorParams generatorParams, IEnumerable<IGeneratorCriteria<LevelSkeleton>> generatorCriteria = null) : base(generatorParams, generatorCriteria)
    {
        var criterias = new List<IGeneratorCriteria<List<SkeletonPoint>>>
        {
            new AvoidNearSkeletonPointsGeneratorCriteria()
        };

        _skeletonPointsGenerator = new SkeletonPointsGenerator(generatorParams.SkeletonPointsGeneratorParams, criterias);
    }

    protected override LevelSkeleton Generate()
    {
        var generatedPoints = new LevelSkeleton();
        var result = new LevelSkeleton();

        //1. Generate random points
        var points = _skeletonPointsGenerator.Execute();
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
        if (Params.AdditionalLinesCount > 0)
            AddAdditionalLines(result, fullSkeleton, basementSkeleton);

        //5. Replace all edges with angle more then 45 degree
        if (Params.ReplaceMoreThan45)
            ReplaceMoreThan45(result);

        //6. Merge points near with line
        if (Params.MergeNearBy)
        {
            MergeNearByPoints(result);
            MergeNearByPointLines(result);
        }

        if (result.IsLinesIntersects())
            return null;

        return result.IsDuplicateLines() ? null : result;
    }

    private void AddAdditionalLines(LevelSkeleton skeleton, LevelSkeleton fullSkeleton, LevelSkeleton basementSkeleton)
    {
        var triangleSkeleton = fullSkeleton.ToTriangleSkeleton();
        var notUsedLines = triangleSkeleton.Lines.Where(f => !basementSkeleton.Lines.Any(r => f.ContainsSkeletonPoint(r.Points.pointA)
                                                                                              && f.ContainsSkeletonPoint(r.Points.pointB))
                                                        ).ToList();

        var i = 0;

        while (i < Params.AdditionalLinesCount)
        {
            var randomLine = notUsedLines[Random.Range(0, notUsedLines.Count - 1)];

            if (skeleton.Lines.Any(_ => _.FindIntersection(randomLine).HasValue))
                continue;

            skeleton.AddLine(randomLine);
            notUsedLines.Remove(randomLine);
            i++;
        }
    }

    private static void ReplaceMoreThan45(LevelSkeleton skeleton)
    {
        var linesToRemove = new List<SkeletonLine>();
        skeleton.Lines.ToList().ForEach(_ =>
        {
            if (!(_.GetLineAngle() > 45)) return;

            var point = new SkeletonPoint(new Vector2(_.Points.pointA.Position.x, _.Points.pointB.Position.y), new EntityType(Color.blue, "Additional"));
            var line1 = new SkeletonLine(_.Points.pointA, point, new EntityType(Color.green, "Corrected"));
            var line2 = new SkeletonLine(_.Points.pointB, point, new EntityType(Color.green, "Corrected"));

            skeleton.AddLine(line1);
            skeleton.AddLine(line2);
            linesToRemove.Add(_);
        });

        skeleton.RemoveLines(linesToRemove);
    }

    private static void MergeNearByPointLines(LevelSkeleton skeleton)
    {
        SkeletonLine lineToRemove1 = null;
        var isStart = true;

        var attempts = 0;

        while ((lineToRemove1 != null || isStart) && attempts <= GeneratorConstants.MaxGenerationAttempts)
        {
            isStart = false;
            lineToRemove1 = null;

            foreach (var point in skeleton.Points)
            {
                var lineToRemove = skeleton.Lines.Where(l => !l.ContainsSkeletonPoint(point) && l.GetDistanceBetweenLineAndPoint(point) < 0.5)
                    .FirstOrDefault();

                if (lineToRemove != null)
                {
                    var l1 = !skeleton.LinesForPoint(point).Any(_ => _.ContainsSkeletonPoint(lineToRemove.Points.pointA))
                        ? new SkeletonLine(point, lineToRemove.Points.pointA)
                        : null;

                    var l2 = !skeleton.LinesForPoint(point).Any(_ => _.ContainsSkeletonPoint(lineToRemove.Points.pointB))
                        ? new SkeletonLine(point, lineToRemove.Points.pointB)
                        : null;

                    if (l1 != null)
                    {
                        skeleton.AddLine(l1);
                        point.Type = new EntityType(Color.red, "NearByLine");
                        lineToRemove1 = lineToRemove;
                    }

                    if (l2 != null)
                    {
                        skeleton.AddLine(l2);
                        point.Type = new EntityType(Color.red, "NearByLine");
                        lineToRemove1 = lineToRemove;
                    }

                    if (lineToRemove1 != null)
                    {
                        break;
                    }
                }
            }

            attempts++;
            skeleton.RemoveLines(new List<SkeletonLine> { lineToRemove1 });
        }

        if (attempts > GeneratorConstants.MaxGenerationAttempts)
        {
            Debug.Log("Too many attempts");
        }
    }

    private static void MergeNearByPoints(LevelSkeleton skeleton)
    {
        var isMerged = true;
        var attempts = 0;

        while (isMerged && attempts < skeleton.Points.Count && attempts < GeneratorConstants.MaxGenerationAttempts)
        {
            attempts++;
            isMerged = false;

            SkeletonPoint pointToMergeA = null;
            SkeletonPoint pointToMergeB = null;

            foreach (var point in skeleton.Points)
            {
                var otherPoint = skeleton.Points.Where(_ => _.Id != point.Id).FirstOrDefault(_ => Vector2.Distance(point.Position, _.Position) < 0.5f);

                if (otherPoint == null) continue;

                pointToMergeA = point;
                pointToMergeB = otherPoint;
                break;
            }

            if (pointToMergeA != null)
            {
                var newLines = skeleton.LinesForPoint(pointToMergeB).Where(_ => !_.ContainsSkeletonPoint(pointToMergeA)).Select(_ => new SkeletonLine(_.Points.pointA.Id == pointToMergeB.Id ? _.Points.pointB : _.Points.pointA, pointToMergeA)).ToList();
                var linesToRemove = skeleton.Lines.Where(_ => _.ContainsSkeletonPoint(pointToMergeB));

                skeleton.RemoveLines(linesToRemove);
                skeleton.RemovePoint(pointToMergeB);
                skeleton.AddLines(newLines);
                isMerged = true;
            }
        }
    }
}
