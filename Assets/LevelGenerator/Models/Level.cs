using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Level
{
    private readonly List<LevelWall> _walls;
    private readonly List<LevelElevator> _elevators;
    private readonly List<Vector2> _rooms;

    public Level()
    {
        _walls = new List<LevelWall>();
        _elevators = new List<LevelElevator>();
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

    public void AddElevator(LevelElevator newElevator)
    {
        AddElevators(new List<LevelElevator> { newElevator });
    }

    public void AddElevators(IEnumerable<LevelElevator> newElevators)
    {
        _elevators.AddRange(newElevators);
    }

    public void AddRooms(IEnumerable<Vector2> newRooms)
    {
        _rooms.AddRange(newRooms);
    }

    public IReadOnlyCollection<LevelWall> Walls => new ReadOnlyCollection<LevelWall>(_walls);
    public IReadOnlyCollection<LevelElevator> Elevators => new ReadOnlyCollection<LevelElevator>(_elevators);
    public IReadOnlyCollection<Vector2> Rooms => new ReadOnlyCollection<Vector2>(_rooms);
}
