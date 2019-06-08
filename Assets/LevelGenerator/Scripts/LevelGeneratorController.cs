using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorController : MonoBehaviour
{
    private LevelSkeleton _levelSkeleton;

    // Start is called before the first frame update
    void Start()
    {
        var levelSkeletonGeneratorParams = new LevelSkeletonGeneratorParams();
        var levelSkeletonGenerator = new LevelSkeletonGenerator(levelSkeletonGeneratorParams);

        _levelSkeleton = levelSkeletonGenerator.Execute();

        var levelSkeletonRenderer = GetComponent<LevelSkeletonRenderer>();
        levelSkeletonRenderer.Draw(_levelSkeleton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
