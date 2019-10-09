using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelExtension
{
    public static Graph<Vector2> ToGraph(this Level level)
    {
        var vertexDictionary = new Dictionary<Vector2, Vertex<Vector2>>();

        level.Walls.ToList().ForEach(_ =>
        {
            if(!vertexDictionary.ContainsKey(_.Points.pointA))
                vertexDictionary.Add(_.Points.pointA, new Vertex<Vector2>(_.Points.pointA));

            if (!vertexDictionary.ContainsKey(_.Points.pointB))
                vertexDictionary.Add(_.Points.pointB, new Vertex<Vector2>(_.Points.pointB));
        });

        var edges = level.Walls.Select(_ => new Edge<Vector2>(vertexDictionary[_.Points.pointA], vertexDictionary[_.Points.pointB], _.Length));

        return new Graph<Vector2>(vertexDictionary.Values.ToList(), edges);
    }
}