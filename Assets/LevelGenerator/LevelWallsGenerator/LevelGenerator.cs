using System.Collections.Generic;

public class LevelGenerator : BaseGenerator<Level, LevelGeneratorParams>
{
    public LevelGenerator(LevelGeneratorParams generatorParams, IEnumerable<IGeneratorCriteria<Level>> generatorCriteria = null) : base(generatorParams, generatorCriteria)
    {
    }

    protected override Level Generate()
    {
        var level = new Level();

        if (_params.LevelSkeleton != null && _params.LevelSkeleton.Lines != null)
        {
            foreach (var line in _params.LevelSkeleton.Lines)
            {
                var walls = line.GetLevelWalls();

                if(walls != null)
                    level.AddWalls(walls);
            }

            foreach (var line in _params.LevelSkeleton.Lines)
            {
                var elevator = line.GetLevelElevator();

                if (elevator != null)
                    level.AddWall(elevator);
            }
        }

        return level;
    }
}
