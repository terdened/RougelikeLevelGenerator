using System;
using System.Collections.Generic;
using System.Linq;

public class Vertex<T>
{
    private readonly List<Edge<T>> _edges;
    private readonly T _data;

    public Vertex(T data)
    {
        Id = Guid.NewGuid().ToId();
        _edges = new List<Edge<T>>();
        _data = data;
        Color = VertexColor.White;
    }

    public Vertex(string id, T data)
    {
        Id = id;
        _edges = new List<Edge<T>>();
        _data = data;
        Color = VertexColor.White;
    }

    public string Id { get; }

    public VertexColor Color { get; set; }

    public int Degree => _edges.Count;

    public List<Edge<T>> Edges => _edges;

    public T Data => _data;

    public List<Vertex<T>> NearVertexes => _edges.Select(_ => _.VertexA == this ? _.VertexB : _.VertexA).ToList();

    public void AddEdge(Edge<T> edge)
    {
        _edges.Add(edge);
    }

    public void RemoveEdge(Edge<T> edge)
    {
        _edges.Remove(edge);
    }
}
