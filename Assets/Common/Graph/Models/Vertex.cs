using System;
using System.Collections.Generic;

public class Vertex<T>
{
    private readonly List<Edge<T>> _edges;
    private readonly T _data;

    public Vertex(T data)
    {
        Id = Guid.NewGuid().ToId();
        _edges = new List<Edge<T>>();
        _data = data;
    }

    public Vertex(string id, T data)
    {
        Id = id;
        _edges = new List<Edge<T>>();
        _data = data;
    }

    public string Id { get; }

    public int Degree => _edges.Count;

    public T Data => _data;

    public void AddEdge(Edge<T> edge)
    {
        _edges.Add(edge);
    }

    public void RemoveEdge(Edge<T> edge)
    {
        _edges.Remove(edge);
    }
}
