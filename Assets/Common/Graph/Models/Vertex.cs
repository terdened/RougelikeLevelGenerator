using System;
using System.Collections.Generic;
using System.Linq;

public class Vertex<T>
{
    public Vertex(T data)
    {
        Id = Guid.NewGuid().ToId();
        Edges = new List<Edge<T>>();
        Data = data;
        Color = VertexColor.White;
    }

    public Vertex(string id, T data)
    {
        Id = id;
        Edges = new List<Edge<T>>();
        Data = data;
        Color = VertexColor.White;
    }

    public string Id { get; }

    public VertexColor Color { get; set; }

    public int Degree => Edges.Count;

    public List<Edge<T>> Edges { get; }

    public T Data { get; }

    public void AddEdge(Edge<T> edge)
    {
        Edges.Add(edge);
    }

    public void RemoveEdge(Edge<T> edge)
    {
        Edges.Remove(edge);
    }
}
