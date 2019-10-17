using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class Graph<T> 
{
    private readonly List<Edge<T>> _edges;
    private readonly List<Vertex<T>> _vertexes;

    public Graph()
    {
        _vertexes = new List<Vertex<T>>();
        _edges = new List<Edge<T>>();
    }

    public Graph(IEnumerable<Vertex<T>> vertexes, IEnumerable<Edge<T>> edges)
    {
        _vertexes = new List<Vertex<T>>();
        _edges = new List<Edge<T>>();
        AddVertexes(vertexes);
        AddEdges(edges);
    }

    public void AddVertex(Vertex<T> newVertex)
    {
        AddVertexes(new List<Vertex<T>> { newVertex });
    }

    public void AddVertexes(IEnumerable<Vertex<T>> newVertexes)
    {
        _vertexes.AddRange(newVertexes);
    }

    public void AddEdge(Edge<T> newEdge)
    {
        AddEdges(new List<Edge<T>> { newEdge });
    }

    public void RemoveEdge(Vertex<T> vertexA, Vertex<T> vertexB)
    {
        _edges.RemoveAll(_ => _.ContainsVertex(vertexA) && _.ContainsVertex(vertexB));
    }

    public void RemoveVertex(Vertex<T> vertex)
    {
        _edges.RemoveAll(_ => _.ContainsVertex(vertex));
        _vertexes.Remove(vertex);
    }

    public void AddEdges(IEnumerable<Edge<T>> newEdges)
    {
        var edges = newEdges as Edge<T>[] ?? newEdges.ToArray();
        _edges.AddRange(edges);

        foreach(var edge in edges)
        {
            foreach(var vertex in edge.Vertexes)
            {
                if (!_vertexes.Contains(vertex))
                    AddVertex(vertex);

                vertex.AddEdge(edge);
            }
        }
    }

    public IReadOnlyCollection<Vertex<T>> Vertexes => new ReadOnlyCollection<Vertex<T>>(_vertexes);

    public IReadOnlyCollection<Edge<T>> Edges => new ReadOnlyCollection<Edge<T>>(_edges);

    public IReadOnlyCollection<Edge<T>> EdgesForVertex(Vertex<T> vertex)
        => new ReadOnlyCollection<Edge<T>>(_edges.Where(_ => _.ContainsVertex(vertex)).ToList());

    public IReadOnlyCollection<Vertex<T>> NearVertexes(Vertex<T> vertex)
        => new ReadOnlyCollection<Vertex<T>>(_edges.Where(_ => _.ContainsVertex(vertex))
            .Select(_ => _.VertexA == vertex ? _.VertexB : _.VertexA).ToList());
}
