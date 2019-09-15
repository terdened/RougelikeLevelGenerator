﻿using System.Collections.Generic;
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
        if (_params.AdditionalLinesCount > 0)
            AddAdditionalLines(result, fullSkeleton, basementSkeleton);

        //5. Replace all edges with angle more then 45 deegree
        if (_params.ReplaceMoreThan45)
            ReplaceMoreThan45(result);

        //6. Merge points near with line
        if (_params.MergeNearBy)
        {
            MergeNearByPoints(result);
            MergeNearByPointLines(result);
        }


        if (result.IsLinesIntersects())
            return null;

        var emptySpaces = FindEmptySpaces(result);
        if (result == null)
            return null;

        DefineLinesTypes(result, emptySpaces);

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

    private void MergeNearByPointLines(LevelSkeleton skeleton)
    {
        var changes = true;

        while (changes)
        {
            changes = false;
            var linesToRemove = new List<SkeletonLine>();
            var isStart = true;

            var attempts = 0;

            while ((linesToRemove.Count > 0 || isStart) && attempts <= GeneratorConstants.MaxGenerationAttemts)
            {
                isStart = false;
                linesToRemove = new List<SkeletonLine>();

                foreach (var point in skeleton.Points)
                {
                    var lineToRemove = skeleton.Lines.Where(l => l.Points.pointA.Id != point.Id && l.Points.pointB.Id != point.Id && l.GetDistanceBetweenLineAndPoint(point) < 0.5)
                        .FirstOrDefault();

                    if (lineToRemove != null)
                    {
                        changes = true;
                        linesToRemove.Add(lineToRemove);

                        var line1 = new SkeletonLine(point, lineToRemove.Points.pointA);
                        var line2 = new SkeletonLine(point, lineToRemove.Points.pointB);

                        skeleton.AddLine(line1);
                        skeleton.AddLine(line2);
                        point.Type = new EntityType(Color.red, "NearByLine");

                        break;
                    }
                }

                attempts++;

                skeleton.RemoveLines(linesToRemove);
            }

            if (attempts > GeneratorConstants.MaxGenerationAttemts)
            {
                Debug.Log("Too many attempts");
                changes = false;
            }
        }
        
    }

    private void MergeNearByPoints(LevelSkeleton skeleton)
    {
        var isMerged = true;
        var attempts = 0;
        while (isMerged && attempts < skeleton.Points.Count)
        {
            attempts++;
            isMerged = false;

            SkeletonPoint pointToMergeA = null;
            SkeletonPoint pointToMergeB = null;

            foreach (var point in skeleton.Points)
            {
                var otherPoint = skeleton.Points.Where(_ => _.Id != point.Id).FirstOrDefault(_ => Vector2.Distance(point.Position, _.Position) < 0.5f);
                if (otherPoint != null)
                {
                    pointToMergeA = point;
                    pointToMergeB = otherPoint;
                    break;
                }
            }

            if (pointToMergeA != null)
            {
                var newLines = pointToMergeB.Lines.Where(_ => !_.ContainsSkeletonPoint(pointToMergeA)).Select(_ => new SkeletonLine(_.Points.pointA.Id == pointToMergeB.Id ? _.Points.pointB : _.Points.pointA, pointToMergeA)).ToList();
                skeleton.RemoveLines(pointToMergeB.Lines);
                skeleton.RemovePoint(pointToMergeB);
                skeleton.AddLines(newLines);
                isMerged = true;
            }
        }
        Debug.Log(attempts);
    }

    private List<List<SkeletonLine>> FindEmptySpaces(LevelSkeleton skeleton)
    {
        var cycles = skeleton.GetCycles();
        cycles.ForEach(cycle => cycle.ForEach(line => line.Type = new EntityType(Color.magenta, "Cycle")));

        cycles.RemoveAll(_ => _.GetPathLength() > _params.MaxOpenSpacePerimeter);

        var emptySpaces = new List<List<SkeletonLine>>();

        var cycleWieght = new Dictionary<List<SkeletonLine>, int>();

        foreach (var cycle in cycles)
        {
            var otherCycles = cycles.Where(_ => !_.IsCycleEquals(cycle));
            var pointACounts = otherCycles.SelectMany(_ => _).Where(_ => cycle.IsSkeletonPointBelongs(_.Points.pointA)).Count();
            var pointBCounts = otherCycles.SelectMany(_ => _).Where(_ => cycle.IsSkeletonPointBelongs(_.Points.pointB)).Count();

            cycleWieght.Add(cycle, pointACounts + pointBCounts);
        }

        while (cycles.Count > 0)
        {
            var maxCycle = cycles.OrderBy(_ => cycleWieght[_]).Last();
            emptySpaces.Add(maxCycle);
            cycles.Remove(maxCycle);

            var subCycles = cycles.Where(_ =>
            {
                foreach (var line in _)
                {
                    if (maxCycle.Any(mc => mc == line))
                        return true;
                }

                return false;
            });

            cycles.RemoveAll(_ => subCycles.Any(sc => _.IsCycleEquals(sc)));
        }

        if (emptySpaces.Any(es => es.Any(l => es.Where(c => c.ContainsSkeletonPoint(l.Points.pointA)).Count() < 2 || es.Where(c => c.ContainsSkeletonPoint(l.Points.pointB)).Count() < 2)))
        {
            skeleton = null;
            return null;
        }

        emptySpaces.RemoveAll(_ => _.GetPathLength() < _params.MinOpenSpacePerimeter);
        emptySpaces.ForEach(emptySpace => emptySpace.ForEach(line => line.Type = new EntityType(Color.cyan, "Empty space")));
        return emptySpaces;
    }

    private void DefineLinesTypes(LevelSkeleton skeleton, List<List<SkeletonLine>> emptySpaces)
    {
        skeleton.Lines.ToList().ForEach(_ =>
        {
            if(_.GetLineAngle() > 45)
            {
                _.Type = new EntityType(new Color(0.33f, 0.24f, 0.21f), "Elevator");
            } else
            {

                _.Type = new EntityType(new Color(0.72f, 0.47f, 0.34f), "Floor");
            }
            _.Points.pointA.Type = new EntityType(new Color(0.72f, 0.47f, 0.34f), "Floor");
            _.Points.pointB.Type = new EntityType(new Color(0.72f, 0.47f, 0.34f), "Floor");
        });

        if (emptySpaces == null)
            return;

        emptySpaces.ForEach(c =>
        {
            c.ForEach(l =>
            {
                var emptySpaceWalls = skeleton.Lines.Where(_ => _.ContainsSkeletonPoint(l.Points.pointA) && _.ContainsSkeletonPoint(l.Points.pointB));
                emptySpaceWalls.ToList().ForEach(_ => {

                    if (_.Type.Name == "Elevator")
                    {
                        var middlePoint = _.GetMiddlePoint();
                        var rightMiddlePoint = new SkeletonPoint(new Vector2(middlePoint.Position.x + 0.1f, middlePoint.Position.y));

                        if (c.IsSkeletonPointInside(rightMiddlePoint))
                        {
                            _.Type = new EntityType(new Color(0.12f, 0.32f, 0.32f), "Empty Space Elevator Left");
                        } else
                        {
                            _.Type = new EntityType(new Color(0.09f, 0.16f, 0.16f), "Empty Space Elevator Right");
                        }

                    } else
                    {
                        var middlePoint = _.GetMiddlePoint();
                        var aboveMiddlePoint = new SkeletonPoint(new Vector2(middlePoint.Position.x, middlePoint.Position.y + 0.1f));
                        if(c.IsSkeletonPointInside(aboveMiddlePoint))
                        {
                            _.Type = new EntityType(new Color(0.0f, 0.5f, 0.5f), "Empty Space Floor");
                        } else
                        {
                            _.Type = new EntityType(Color.cyan, "Empty Space Roof");
                        }
                    }
                });
                l.Points.pointA.Type = new EntityType(Color.cyan, "Empty Space");
                l.Points.pointB.Type = new EntityType(Color.cyan, "Empty Space");
            });

            var linesInEmptySpace = skeleton.Lines.Where(_ => !c.Any(l => l.ContainsSkeletonPoint(_.Points.pointA)) && c.IsSkeletonPointInside(_.Points.pointA)
            || !c.Any(l => l.ContainsSkeletonPoint(_.Points.pointB)) && c.IsSkeletonPointInside(_.Points.pointB)).ToList();

            linesInEmptySpace.AddRange(skeleton.Lines.Where(_ => !c.Any(l => l.ContainsSkeletonPoint(_.Points.pointA) && l.ContainsSkeletonPoint(_.Points.pointB))
                && c.Any(l => l.ContainsSkeletonPoint(_.Points.pointA)) && c.Any(l => l.ContainsSkeletonPoint(_.Points.pointB))));
            
            linesInEmptySpace.ToList().ForEach(l =>
            {
                if (l.Type.Name == "Elevator")
                {
                    l.Type = new EntityType(new Color(0.25f, 0.25f, 0.25f), "Elevator Inside");
                } else
                {
                    l.Type = new EntityType(Color.grey, "Inside Floor");
                }
                l.Points.pointA.Type = new EntityType(Color.grey, "Inside Floor");
                l.Points.pointB.Type = new EntityType(Color.grey, "Inside Floor");
            });
        });
    }
}
