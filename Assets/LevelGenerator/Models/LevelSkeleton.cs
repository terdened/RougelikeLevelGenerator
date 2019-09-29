using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class LevelSkeleton
{
    private readonly List<SkeletonLine> _lines;
    private readonly List<SkeletonPoint> _points;

    public LevelSkeleton()
    {
        _lines = new List<SkeletonLine>();
        _points = new List<SkeletonPoint>();
    }

    public void AddPoint(SkeletonPoint newPoint)
    {
        AddPoints(new List<SkeletonPoint> { newPoint });
    }

    public void RemovePoint(SkeletonPoint point)
    {
        _points.RemoveAll(_ => _.Id == point.Id);
    }

    public void AddPoints(IEnumerable<SkeletonPoint> newPoints)
    {
        _points.AddRange(newPoints);
    }

    public void AddLine(SkeletonLine newLine)
    {
        AddLines(new List<SkeletonLine> { newLine });
    }

    public void AddLines(IEnumerable<SkeletonLine> newLines)
    {
        var skeletonLines = newLines.ToList();
        _lines.AddRange(skeletonLines);

        foreach (var line in skeletonLines)
        {
            foreach (var point in line.PointsList)
            {
                if (!_points.Contains(point))
                    AddPoint(point);
            }
        }
    }

    public void RemoveLines(IEnumerable<SkeletonLine> lines)
    {
        if (lines == null)
            return;

        _lines.RemoveAll(lines.Contains);
    }

    public void SetLineType(EntityType type)
    {
        _lines.ForEach(_ => _.Type = type);
    }

    public IReadOnlyCollection<SkeletonLine> Lines => new ReadOnlyCollection<SkeletonLine>(_lines);

    public IReadOnlyCollection<SkeletonPoint> Points => new ReadOnlyCollection<SkeletonPoint>(_points);

    public IReadOnlyCollection<SkeletonLine> LinesForPoint(SkeletonPoint point) 
        => new ReadOnlyCollection<SkeletonLine>(_lines.Where(_ => _.ContainsSkeletonPoint(point)).ToList());
}
