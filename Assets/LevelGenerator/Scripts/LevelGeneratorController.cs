using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorController : MonoBehaviour
{
    private LevelSkeleton _levelSkeleton;
    private Level _level;

    // Start is called before the first frame update
    void Start()
    {
        var levelSkeletonGeneratorParams = new LevelSkeletonGeneratorParams();
        Generate(levelSkeletonGeneratorParams);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate(LevelSkeletonGeneratorParams levelSkeletonGeneratorParams)
    {
        var levelSkeletonGenerator = new LevelSkeletonGenerator(levelSkeletonGeneratorParams);

        _levelSkeleton = levelSkeletonGenerator.Execute();

        if (_levelSkeleton == null)
        {
            Debug.Log("[Failed] generation failed");
            return;
        }

        var levelSkeletonRenderer = GetComponent<LevelSkeletonRenderer>();
        // levelSkeletonRenderer.Draw(_levelSkeleton);

        var levelGeneratorParams = new LevelGeneratorParams()
        {
            LevelSkeleton = _levelSkeleton
        };

        var levelGenerator = new LevelGenerator(levelGeneratorParams);

        _level = levelGenerator.Execute();

        if (_level == null)
        {
            Debug.Log("[Failed] Level generation failed");
            return;
        }

        var levelRenderer = GetComponent<LevelRenderer>();
        levelRenderer.Draw(_level);
    }
}
