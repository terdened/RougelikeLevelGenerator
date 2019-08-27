using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GraphExtension
{
    public static Graph<T> ToFullGraph<T>(this Graph<T> graph)
    {
        foreach(var vertex in graph.Vertexes)
        {
            var vertexesForEdges = graph.Vertexes.Where(_ => _ != vertex && !graph.Edges.Any(e =>
                    (e.VertexA == vertex && e.VertexB == _) || (e.VertexB == vertex && e.VertexA == _)));

            var newEdges = vertexesForEdges.Select(_ => new Edge<T>(vertex, _, 1));
            graph.AddEdges(newEdges);
        }

        return graph;
    }

    public static Graph<T> AlgorithmByPrim<T>(this Graph<T> graph)
    {
        int numberV = graph.Vertexes.Count;

        var result = new Graph<T>();
        result.AddVertexes(graph.Vertexes.ToList().Select(_ =>
        {
            return new Vertex<T>(_.Id, _.Data);
        }));

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
            int minE = -1; //номер наименьшего ребра
                           //поиск наименьшего ребра
            for (int i = 0; i < notUsedE.Count; i++)
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
        var result = dfs<T>(graph.Vertexes.First(), graph, null, null);
        return result;
    }

    private static List<List<Edge<T>>> dfs<T>(Vertex<T> vertex, Graph<T> graph, List<Edge<T>> currentCycle, List<List<Edge<T>>> result)
    {
        if (result == null)
            result = new List<List<Edge<T>>>();
        
        if (currentCycle == null)
            currentCycle = new List<Edge<T>>();

        vertex.Color = VertexColor.Grey;

        foreach (var nearVertex in vertex.NearVertexes)
        {
            if (currentCycle.Count > 0 && (currentCycle.Last().VertexA == nearVertex || currentCycle.Last().VertexB == nearVertex))
                continue;

            if (nearVertex.Color == VertexColor.White)
            {
                var newCurrentCycle = currentCycle.Select(_ => _).ToList();
                newCurrentCycle.Add(vertex.Edges.First(_ => _.VertexA == nearVertex || _.VertexB == nearVertex));
                
                dfs<T>(nearVertex, graph, newCurrentCycle, result);
            }
            else if (nearVertex.Color == VertexColor.Grey)
            {
                var newCurrentCycle = currentCycle.Select(_ => _).ToList();
                var firstVertexIndex = currentCycle.IndexOf(currentCycle.First(_ => _.VertexA == nearVertex || _.VertexB == nearVertex));


                newCurrentCycle = newCurrentCycle.Skip(firstVertexIndex).ToList();

                //if (!(newCurrentCycle[firstVertexIndex].VertexA == nearVertex && newCurrentCycle[firstVertexIndex].VertexB == vertex)
                //    && !(newCurrentCycle[firstVertexIndex].VertexB == nearVertex && newCurrentCycle[firstVertexIndex].VertexA == vertex))
                //{
                //    newCurrentCycle = newCurrentCycle.Skip(firstVertexIndex + 1).ToList();
                //}

                newCurrentCycle.Add(vertex.Edges.First(_ => _.VertexA == nearVertex || _.VertexB == nearVertex));

                var notCycleEdge = newCurrentCycle.Where(_ => newCurrentCycle.Count(a => a.VertexA == _.VertexA || a.VertexB == _.VertexA) < 2 || newCurrentCycle.Count(a => a.VertexA == _.VertexB || a.VertexB == _.VertexB) < 2);

                newCurrentCycle.RemoveAll(_ => notCycleEdge.Contains(_));
                result.Add(newCurrentCycle);
            }
        }

        vertex.Color = VertexColor.Black;
        return result;
    }
}
