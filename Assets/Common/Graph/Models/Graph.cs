using System.Collections.Generic;
using System.Collections.ObjectModel;

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

    public void AddEdges(IEnumerable<Edge<T>> newEdges)
    {
        _edges.AddRange(newEdges);

        foreach(var edge in newEdges)
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
}
