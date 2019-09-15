using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class Level
{
    private readonly List<LevelWall> _walls;

    public Level()
    {
        _walls = new List<LevelWall>();
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

    public IReadOnlyCollection<LevelWall> Walls => new ReadOnlyCollection<LevelWall>(_walls);
}
