using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class Level
{
    private readonly List<LevelWall> _walls;
    private readonly List<Vector2> _rooms;

    public Level()
    {
        _walls = new List<LevelWall>();
        _rooms = new List<Vector2>();
    }

    public void AddWall(LevelWall newWall)
    {
        AddWalls(new List<LevelWall> { newWall });
    }

    public void AddWalls(IEnumerable<LevelWall> newWalls)
    {
        _walls.AddRange(newWalls);
    }

    public void RemoveWalls(IEnumerable<LevelWall> walls)
    {
        _walls.RemoveAll(_ => walls.Any(l => _.Id == l.Id));
    }

    public void AddRooms(IEnumerable<Vector2> newRooms)
    {
        _rooms.AddRange(newRooms);
    }

    public IReadOnlyCollection<LevelWall> Walls => new ReadOnlyCollection<LevelWall>(_walls);
    public IReadOnlyCollection<Vector2> Rooms => new ReadOnlyCollection<Vector2>(_rooms);
}
