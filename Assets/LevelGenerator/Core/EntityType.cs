using UnityEngine;

public struct EntityType
{
    private readonly Color _color;
    private readonly string _name;
    
    public Color Color => _color == default ? Color.gray : _color;
    public string Name => _name ?? "Unknown";

    public EntityType(Color color, string name)
    {
        _color = color;
        _name = name;
    }

    public static bool operator ==(EntityType t1, EntityType t2)
    {
        return t1._name == t2._name;
    }

    public static bool operator !=(EntityType t1, EntityType t2)
    {
        return t1._name != t2._name;
    }
}
