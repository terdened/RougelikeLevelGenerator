using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class Edge<T>
{
    private (Vertex<T> vertexA, Vertex<T> vertexB) _vertexes;

    public Edge(Vertex<T> vertexA, Vertex<T> vertexB, float length)
    {
        Id = Guid.NewGuid().ToId();
        _vertexes = (vertexA, vertexB);
        Length = length;
    }

    public string Id { get; }

    public float Length { get; }

    public IReadOnlyCollection<Vertex<T>> Vertexes => new ReadOnlyCollection<Vertex<T>>(new List<Vertex<T>> { _vertexes.vertexA, _vertexes.vertexB });

    public Vertex<T> VertexA => _vertexes.vertexA;

    public Vertex<T> VertexB => _vertexes.vertexB;
}
