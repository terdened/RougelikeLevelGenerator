using System;
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
}