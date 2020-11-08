using Assets.LevelGenerator.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class Level
{
    private readonly List<LevelWall> _walls;
    private readonly List<LevelElevator> _elevators;
    private readonly List<LevelRoom> _rooms;
    private readonly List<LevelJumppad> _jumppads;
    private readonly List<LevelExit> _exits;

    public Level()
    {
        _walls = new List<LevelWall>();
        _elevators = new List<LevelElevator>();
        _rooms = new List<LevelRoom>();
        _jumppads = new List<LevelJumppad>();
        _exits = new List<LevelExit>();
    }

    public void AddJumppad(LevelJumppad newJumppad)
    {
        _jumppads.Add(newJumppad);
    }

    public void AddExit(LevelExit exit)
    {
        _exits.Add(exit);
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

    public void AddRoom(Vector2 position, string type)
    {
        _rooms.Add(new LevelRoom {
            Position = position,
            Type = type
        });
    }

    public void RemoveWall(string wallId)
    {
        _walls.RemoveAll(_ => _.Id == wallId);
    }

    public IReadOnlyCollection<LevelWall> Walls => new ReadOnlyCollection<LevelWall>(_walls);
    public IReadOnlyCollection<LevelElevator> Elevators => new ReadOnlyCollection<LevelElevator>(_elevators);
    public IReadOnlyCollection<LevelRoom> Rooms => new ReadOnlyCollection<LevelRoom>(_rooms);
    public IReadOnlyCollection<LevelJumppad> Jumppads => new ReadOnlyCollection<LevelJumppad>(_jumppads);
    public IReadOnlyCollection<LevelExit> Exits => new ReadOnlyCollection<LevelExit>(_exits);
}
