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
        if (Params.LevelSkeleton == null)
            return null;

        var emptySpaces = FindEmptySpaces(Params.LevelSkeleton);
        if (Params.LevelSkeleton == null)
            return null;

        DefineLinesTypes(Params.LevelSkeleton, emptySpaces);

        return Params.LevelSkeleton;
    }

    private List<List<SkeletonLine>> FindEmptySpaces(LevelSkeleton skeleton)
    {
        var cycles = skeleton.GetCycles();
        cycles.ForEach(cycle => cycle.ForEach(line => line.Type = new EntityType(Color.magenta, "Cycle")));

        cycles.RemoveAll(_ => _.GetPathLength() > Params.MaxOpenSpacePerimeter);

        var emptySpaces = new List<List<SkeletonLine>>();

        var cycleWeight = new Dictionary<List<SkeletonLine>, int>();

        foreach (var cycle in cycles)
        {
            var otherCycles = cycles.Where(_ => !_.IsCycleEquals(cycle)).ToList();
            var pointACounts = otherCycles.SelectMany(_ => _).Count(_ => cycle.IsSkeletonPointBelongs(_.Points.pointA));
            var pointBCounts = otherCycles.SelectMany(_ => _).Count(_ => cycle.IsSkeletonPointBelongs(_.Points.pointB));

            cycleWeight.Add(cycle, pointACounts + pointBCounts);
        }

        while (cycles.Count > 0)
        {
            var maxCycle = cycles.OrderBy(_ => cycleWeight[_]).Last();
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

            cycles.RemoveAll(_ => subCycles.Any(_.IsCycleEquals));
        }

        if (emptySpaces.Any(es => es.Any(l => es.Count(c => c.ContainsSkeletonPoint(l.Points.pointA)) < 2 
                                              || es.Count(c => c.ContainsSkeletonPoint(l.Points.pointB)) < 2)))
        {
            skeleton = null;
            return null;
        }

        emptySpaces.RemoveAll(_ => _.GetPathLength() < Params.MinOpenSpacePerimeter);
        emptySpaces.ForEach(emptySpace => emptySpace.ForEach(line => line.Type = new EntityType(Color.cyan, "Empty space")));
        return emptySpaces;
    }

    private static void DefineLinesTypes(LevelSkeleton skeleton, List<List<SkeletonLine>> emptySpaces)
    {
        skeleton.Lines.ToList().ForEach(_ =>
        {
            _.Type = _.GetLineAngle() > 45 ? EntityTypeConstants.Elevator : EntityTypeConstants.Floor;

            //_.Points.pointA.Type = EntityTypeConstants.Floor;
            //_.Points.pointB.Type = EntityTypeConstants.Floor;
        });

        emptySpaces?.ForEach(c =>
        {
            c.ForEach(l =>
            {
                var emptySpaceWalls = skeleton.Lines.Where(_ => _.ContainsSkeletonPoint(l.Points.pointA) && _.ContainsSkeletonPoint(l.Points.pointB));
                emptySpaceWalls.ToList().ForEach(_ => {

                    if (_.Type.Name == "Elevator")
                    {
                        var middlePoint = _.GetMiddlePoint();
                        var rightMiddlePoint = new SkeletonPoint(new Vector2(middlePoint.Position.x + 0.1f, middlePoint.Position.y));

                        _.Type = c.IsSkeletonPointInsideStatistics(rightMiddlePoint) 
                            ? EntityTypeConstants.EmptySpaceElevatorLeft 
                            : EntityTypeConstants.EmptySpaceElevatorRight;

                    } else
                    {
                        var middlePoint = _.GetMiddlePoint();
                        var aboveMiddlePoint = new SkeletonPoint(new Vector2(middlePoint.Position.x, middlePoint.Position.y + 0.1f));
                        _.Type = c.IsSkeletonPointInsideStatistics(aboveMiddlePoint) 
                            ? EntityTypeConstants.EmptySpaceFloor 
                            : EntityTypeConstants.EmptySpaceTop;
                    }
                });
                //l.Points.pointA.Type = EntityTypeConstants.EmptySpace;
                //l.Points.pointB.Type = EntityTypeConstants.EmptySpace;
            });

            var linesInEmptySpace = skeleton.Lines.Where(_ => !c.Any(l => l.ContainsSkeletonPoint(_.Points.pointA)) && c.IsSkeletonPointInsideStatistics(_.Points.pointA)
                                                              || !c.Any(l => l.ContainsSkeletonPoint(_.Points.pointB)) && c.IsSkeletonPointInsideStatistics(_.Points.pointB)).ToList();

            linesInEmptySpace.AddRange(skeleton.Lines.Where(_ => !c.Any(l => l.ContainsSkeletonPoint(_.Points.pointA) && l.ContainsSkeletonPoint(_.Points.pointB))
                                                                 && c.Any(l => l.ContainsSkeletonPoint(_.Points.pointA)) && c.Any(l => l.ContainsSkeletonPoint(_.Points.pointB))));
            
            linesInEmptySpace.ToList().ForEach(l =>
            {
                l.Type = l.Type.Name == "Elevator" ? EntityTypeConstants.InsideElevator : EntityTypeConstants.InsideFloor;
                //l.Points.pointA.Type = EntityTypeConstants.InsideFloor;
                //l.Points.pointB.Type = EntityTypeConstants.InsideFloor;
            });
        });
    }
}
