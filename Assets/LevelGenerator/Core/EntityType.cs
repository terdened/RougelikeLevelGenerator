
using UnityEngine;

public class EntityType
{
    public Color Color { get; set; }

    public string Name { get; set; }

    public EntityType()
    {
        Color = Color.gray;
        Name = "Unknown";
    }

    public EntityType(Color color, string name)
    {
        Color = color;
        Name = name;
    }
}
