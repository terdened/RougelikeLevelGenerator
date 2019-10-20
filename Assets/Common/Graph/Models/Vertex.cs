using System;

public class Vertex<T>
{
    public Vertex(T data)
    {
        Id = Guid.NewGuid().ToId();
        Data = data;
        Color = VertexColor.White;
    }

    public Vertex(string id, T data)
    {
        Id = id;
        Data = data;
        Color = VertexColor.White;
    }

    public string Id { get; }

    public VertexColor Color { get; set; }

    public T Data { get; }
}
