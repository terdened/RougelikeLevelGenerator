using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GraphExtension
{
    public static Graph<T> ToFullGraph<T>(this Graph<T> graph)
    {
        foreach(var vertex in graph.Vertexes)
        {
            var vertexesForEdges = graph.Vertexes.Where(v => v != vertex && !graph.Edges.Any(e => e.ContainsVertex(v) && e.ContainsVertex(vertex)));
            var newEdges = vertexesForEdges.Select(_ => new Edge<T>(vertex, _, 1));
            graph.AddEdges(newEdges);
        }

        return graph;
    }

    public static Graph<T> AlgorithmByPrim<T>(this Graph<T> graph)
    {
        var numberV = graph.Vertexes.Count;

        var result = new Graph<T>();
        result.AddVertexes(graph.Vertexes.ToList().Select(_ => new Vertex<T>(_.Id, _.Data)));

        //неиспользованные ребра
        var notUsedE = graph.Edges.ToList();
        //использованные вершины
        var usedV = new List<Vertex<T>>();
        //неиспользованные вершины
        var notUsedV = graph.Vertexes.ToList();

        //выбираем случайную начальную вершину
        usedV.Add(graph.Vertexes.ToList()[Random.Range(0, numberV)]);
        notUsedV.Remove(usedV[0]);

        while (notUsedV.Count > 0)
        {
            var minE = -1; //номер наименьшего ребра
                           //поиск наименьшего ребра
            for (var i = 0; i < notUsedE.Count; i++)
            {
                if (usedV.Any(_ => _ == notUsedE[i].VertexA) && notUsedV.Any(_ => _ == notUsedE[i].VertexB) ||
                    usedV.Any(_ => _ == notUsedE[i].VertexB) && notUsedV.Any(_ => _ == notUsedE[i].VertexA))
                {
                    if (minE != -1)
                    {
                        if (notUsedE[i].Length < notUsedE[minE].Length)
                            minE = i;
                    }
                    else
                        minE = i;
                }
            }

            //заносим новую вершину в список использованных и удаляем ее из списка неиспользованных
            if (usedV.Any(_ => _ == notUsedE[minE].VertexA))
            {
                usedV.Add(notUsedE[minE].VertexB);
                notUsedV.Remove(notUsedE[minE].VertexB);
            }
            else
            {
                usedV.Add(notUsedE[minE].VertexA);
                notUsedV.Remove(notUsedE[minE].VertexA);
            }
            //заносим новое ребро в дерево и удаляем его из списка неиспользованных

            result.AddEdge(new Edge<T>(notUsedE[minE].VertexA, notUsedE[minE].VertexB, notUsedE[minE].Length));
            notUsedE.RemoveAt(minE);
        }

        return result;
    }

    public static List<List<Edge<T>>> GetCycles<T>(this Graph<T> graph)
    {
        var result = Dfs<T>(graph, graph.Vertexes.First(), (List<Edge<T>>)null, (List<List<Edge<T>>>)null);
        return result;
    }

    public static (Vector2 pointA, Vector2 pointB) ToVector2(this Edge<Vector2> edge)
        => (edge.VertexA.Data, edge.VertexB.Data);

    public static bool IsVertexInside(this List<Edge<Vector2>> perimeter, Vertex<Vector2> point)
        => perimeter.IsPointBelongs(point);

    public static bool IsPointBelongs(this List<Edge<Vector2>> perimeter, Vertex<Vector2> point)
        => perimeter.Any(l => l.ToVector2().ContainsPoint(point.Data)) || perimeter.Select(_ => _.ToVector2()).ToList().IsPointInside(point.Data);

    public static bool IsCycleEquals(this List<Edge<Vector2>> cycleA, List<Edge<Vector2>> cycleB)
    {
        if (cycleA.Count() != cycleB.Count())
            return false;

        var result = true;

        cycleA.ForEach(_ =>
        {
            if (!cycleB.Contains(_))
                result = false;
        });

        return result;
    }

    public static List<Vertex<T>> FindWay<T>(this Graph<T> graph, Vertex<T> fromVertex, Vertex<T> toVertex)
    {
        // Reset vertexes colors
        // TODO: Move to graph
        foreach (var graphVertex in graph.Vertexes)
        {
            graphVertex.Color = VertexColor.White;
        }

        var result = Dfs<T>(graph, fromVertex, toVertex, null);
        return result;
    }

    private static List<List<Edge<T>>> Dfs<T>(Graph<T> graph, Vertex<T> vertex, List<Edge<T>> currentCycle, List<List<Edge<T>>> result)
    {
        if (result == null)
            result = new List<List<Edge<T>>>();
        
        if (currentCycle == null)
            currentCycle = new List<Edge<T>>();

        vertex.Color = VertexColor.Grey;

        foreach (var nearVertex in graph.NearVertexes(vertex))
        {
            if (currentCycle.Count > 0 && (currentCycle.Last().ContainsVertex(nearVertex)))
                continue;

            if(currentCycle.Any(_ => _.ContainsVertex(nearVertex)))
            {
                var newCurrentCycle = currentCycle.Select(_ => _).ToList();
                var firstVertexIndex = currentCycle.IndexOf(currentCycle.First(_ => _.ContainsVertex(nearVertex)));

                newCurrentCycle = newCurrentCycle.Skip(firstVertexIndex).ToList();
                newCurrentCycle.Add(graph.EdgesForVertex(vertex).First(_ => _.ContainsVertex(nearVertex)));

                var notCycleEdge = newCurrentCycle.Where(_ =>
                                                             newCurrentCycle.Count(a => a.ContainsVertex(_.VertexA)) < 2
                                                             || newCurrentCycle.Count(a => a.ContainsVertex(_.VertexB)) < 2);
                newCurrentCycle.RemoveAll(_ => notCycleEdge.Contains(_));

                result.Add(newCurrentCycle);
            } else
            {
                var newCurrentCycle = currentCycle.Select(_ => _).ToList();
                newCurrentCycle.Add(graph.EdgesForVertex(vertex).First(_ => _.ContainsVertex(nearVertex)));

                Dfs<T>(graph, nearVertex, newCurrentCycle, result);
            }
        }

        vertex.Color = VertexColor.Black;
        return result;
    }

    private static List<Vertex<T>> Dfs<T>(Graph<T> graph, Vertex<T> currentVertex, Vertex<T> goalVertex, List<Vertex<T>> path)
    {
        if(path == null)
            path = new List<Vertex<T>>{ currentVertex };

        currentVertex.Color = VertexColor.Grey;
        var near = graph.NearVertexes(currentVertex).Where(_ => _.Color != VertexColor.Grey);

        foreach (var nearVertex in near)
        {
            if (goalVertex == nearVertex)
            {
                path.Add(nearVertex);
                return path;
            }

            var newPath = path.ToList();
            newPath.Add(nearVertex);

            return Dfs<T>(graph, nearVertex, goalVertex, newPath);
        }

        return null;
    }
}
