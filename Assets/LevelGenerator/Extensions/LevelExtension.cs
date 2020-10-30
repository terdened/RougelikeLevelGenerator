using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LevelExtension
{
    public static Graph<Vector2> ToGraph(this Level level)
    {
        //var vertexDictionary = new Dictionary<string, Vertex<Vector2>>();
        var vertexList = new List<Vertex<Vector2>>();

        //level.Walls.ToList().ForEach(_ =>
        //{
        //    if(!vertexDictionary.ContainsKey(_.Points.pointA.ToString2()))
        //        vertexDictionary.Add(_.Points.pointA.ToString2(), new Vertex<Vector2>(_.Points.pointA));

        //    if (!vertexDictionary.ContainsKey(_.Points.pointB.ToString2()))
        //        vertexDictionary.Add(_.Points.pointB.ToString2(), new Vertex<Vector2>(_.Points.pointB));
        //});

        level.Walls.ToList().ForEach(_ =>
        {
            if (!vertexList.Any(v => Vector2.Distance(v.Data, _.Points.pointA) < 0.001f))
                vertexList.Add(new Vertex<Vector2>(_.Points.pointA));


            if (!vertexList.Any(v => Vector2.Distance(v.Data, _.Points.pointB) < 0.001f))
                vertexList.Add(new Vertex<Vector2>(_.Points.pointB));
        });

        // var edges = level.Walls.Select(_ => new Edge<Vector2>(vertexDictionary[_.Points.pointA.ToString2()], vertexDictionary[_.Points.pointB.ToString2()], _.Length));


        var edges = level.Walls.Select(
            _ => new Edge<Vector2>(vertexList.First(
                v => Vector2.Distance(v.Data, _.Points.pointA) < 0.001f), 
                vertexList.First(v => Vector2.Distance(v.Data, _.Points.pointB) < 0.001f),
                _.Length)
        );

        return new Graph<Vector2>(vertexList, edges);
    }
}