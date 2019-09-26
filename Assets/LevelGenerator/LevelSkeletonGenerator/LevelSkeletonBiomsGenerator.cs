using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelSkeletonBiomsGenerator : BaseGenerator<LevelSkeleton, LevelSkeletonBiomsGeneratorParams>
{
    public LevelSkeletonBiomsGenerator(LevelSkeletonBiomsGeneratorParams generatorParams, IEnumerable<IGeneratorCriteria<LevelSkeleton>> generatorCriteria = null) : base(generatorParams, generatorCriteria)
    {
    }

    protected override LevelSkeleton Generate()
    {
        if (_params.LevelSkeleton == null)
            return null;

        var emptySpaces = FindEmptySpaces(_params.LevelSkeleton);
        if (_params.LevelSkeleton == null)
            return null;

        DefineLinesTypes(_params.LevelSkeleton, emptySpaces);

        return _params.LevelSkeleton;
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
                _.Type = EntityTypeConstants.Elevator;
            } else
            {

                _.Type = EntityTypeConstants.Floor;
            }
            _.Points.pointA.Type = EntityTypeConstants.Floor;
            _.Points.pointB.Type = EntityTypeConstants.Floor;
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

                        if (c.IsSkeletonPointInsideStatistics(rightMiddlePoint))
                        {
                            _.Type = EntityTypeConstants.EmptySpaceElevatorLeft;
                        } else
                        {
                            _.Type = EntityTypeConstants.EmptySpaceElevatorRight;
                        }

                    } else
                    {
                        var middlePoint = _.GetMiddlePoint();
                        var aboveMiddlePoint = new SkeletonPoint(new Vector2(middlePoint.Position.x, middlePoint.Position.y + 0.1f));
                        if(c.IsSkeletonPointInsideStatistics(aboveMiddlePoint))
                        {
                            _.Type = EntityTypeConstants.EmptySpaceFloor;
                        } else
                        {
                            _.Type = EntityTypeConstants.EmptySpaceTop;
                        }
                    }
                });
                l.Points.pointA.Type = EntityTypeConstants.EmptySpace;
                l.Points.pointB.Type = EntityTypeConstants.EmptySpace;
            });

            var linesInEmptySpace = skeleton.Lines.Where(_ => !c.Any(l => l.ContainsSkeletonPoint(_.Points.pointA)) && c.IsSkeletonPointInsideStatistics(_.Points.pointA)
            || !c.Any(l => l.ContainsSkeletonPoint(_.Points.pointB)) && c.IsSkeletonPointInsideStatistics(_.Points.pointB)).ToList();

            linesInEmptySpace.AddRange(skeleton.Lines.Where(_ => !c.Any(l => l.ContainsSkeletonPoint(_.Points.pointA) && l.ContainsSkeletonPoint(_.Points.pointB))
                && c.Any(l => l.ContainsSkeletonPoint(_.Points.pointA)) && c.Any(l => l.ContainsSkeletonPoint(_.Points.pointB))));
            
            linesInEmptySpace.ToList().ForEach(l =>
            {
                if (l.Type.Name == "Elevator")
                {
                    l.Type = EntityTypeConstants.InsideElevator;
                } else
                {
                    l.Type = EntityTypeConstants.InsideFloor;
                }
                l.Points.pointA.Type = EntityTypeConstants.InsideFloor;
                l.Points.pointB.Type = EntityTypeConstants.InsideFloor;
            });
        });
    }
}
