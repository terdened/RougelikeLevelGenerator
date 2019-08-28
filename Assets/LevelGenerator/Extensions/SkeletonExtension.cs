using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class SkeletonExtension
{
    public static Graph<SkeletonPoint> ToGraph(this LevelSkeleton skeleton)
    {
        var vertexDictionary = new Dictionary<string, Vertex<SkeletonPoint>>();

        skeleton.Points.ToList().ForEach(_ =>
        {
            vertexDictionary.Add(_.Id, new Vertex<SkeletonPoint>(_));
        });

        var edges = skeleton.Lines.Select(_ => new Edge<SkeletonPoint>(vertexDictionary[_.Points.pointA.Id], vertexDictionary[_.Points.pointB.Id], _.Length));

        return new Graph<SkeletonPoint>(vertexDictionary.Values.ToList(), edges);
    }

    public static LevelSkeleton ToTriangleSkeleton(this LevelSkeleton skeleton)
    {
        var fullSkeleton = new LevelSkeleton();
        fullSkeleton.AddLines(skeleton.Lines);

        var linesToRemove = new List<SkeletonLine>();
        var isFirst = true;

        while (isFirst || linesToRemove.Count > 0)
        {
            isFirst = false;
            linesToRemove = new List<SkeletonLine>();
            var lines = fullSkeleton.Lines.OrderBy(_ => _.Length).ToList();

            foreach (var line in lines)
            {
                var l = fullSkeleton.Lines.Except(new[] { line })
                                          .Except(linesToRemove)
                                          .Where(_ => _.FindIntersection(line) != null && _.Length > line.Length)
                                          .ToList();

                if (l.Count > 0)
                {
                    l = l.OrderBy(_ => _.Length).ToList();
                    linesToRemove.Add(l.Last());
                    break;
                }

                linesToRemove.AddRange(l.Except(linesToRemove));
            }

            if (linesToRemove.Count > 0)
                fullSkeleton.RemoveLines(linesToRemove);
        }

        fullSkeleton.SetLineType(new EntityType(Color.red, "Additional"));

        return fullSkeleton;
    }

    public static LevelSkeleton ToBasementSkeleton(this LevelSkeleton skeleton)
    {
        var basementSkeleton = new LevelSkeleton();

        var newLines = skeleton.ToGraph().AlgorithmByPrim().Edges.ToList().Select(_ =>
        {
            return new SkeletonLine(_.Vertexes.ToList()[0].Data, _.Vertexes.ToList()[1].Data, new EntityType(Color.blue, "Basement"));
        });

        basementSkeleton.AddLines(newLines);

        return basementSkeleton;
    }

    public static void Draw(this LevelSkeleton skeleton)
    {
        skeleton.Lines.ToList().ForEach(_ =>
        {
            Debug.DrawLine(_.Points.pointA.Position, _.Points.pointB.Position, Color.green);
        });

        skeleton.Points.ToList().ForEach(_ =>
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(_.Position, 0.1f);
        });
    }

    public static List<List<SkeletonLine>> GetCycles(this LevelSkeleton skeleton)
    {
        var cycles = new List<List<SkeletonLine>>();

        var graphCycles = skeleton.ToGraph().GetCycles();

        foreach(var graphCycle in graphCycles)
        {
            var cycle = new List<SkeletonLine>();
            graphCycle.ForEach(edge => cycle.Add(skeleton.Lines.First(_ => _.ContainsSkeletonPoint(edge.VertexA.Data) && _.ContainsSkeletonPoint(edge.VertexB.Data))));
            cycles.Add(cycle);
        }

        return cycles;
    }
}