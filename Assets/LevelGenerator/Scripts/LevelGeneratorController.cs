using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGeneratorController : MonoBehaviour
{
    private LevelSkeleton _levelSkeleton;
    private LevelSkeleton _levelSkeletonWithBioms;
    private Level _level;

    private List<GameObject> _colliders;

    private LevelSkeletonRenderer _skeletonRenderer;
    private LevelRenderer _levelRenderer;

    public Toggle ShowSkeleton;
    public Toggle ShowWalls;
    
    void Start()
    {
        Init();

        ShowSkeleton.onValueChanged.AddListener(delegate {
            Redraw();
        });
        ShowWalls.onValueChanged.AddListener(delegate {
            Redraw();
        });

        var levelSkeletonGeneratorParams = new LevelSkeletonGeneratorParams();
        Regenerate(levelSkeletonGeneratorParams);
    }

    void Init()
    {
        if(_skeletonRenderer == null)
        {
            _skeletonRenderer = GetComponent<LevelSkeletonRenderer>();
        }

        if (_levelRenderer == null)
        {
            _levelRenderer = GetComponent<LevelRenderer>();
        }
    }

    public void Regenerate(LevelSkeletonGeneratorParams levelSkeletonGeneratorParams)
    {
        _levelSkeleton = null;
        _levelSkeletonWithBioms = null;
        _level = null;

        var levelSkeletonGenerator = new LevelSkeletonGenerator(levelSkeletonGeneratorParams);

        _levelSkeleton = levelSkeletonGenerator.Execute();

        if (_levelSkeleton == null)
        {
            Debug.Log("[Failed] generation failed");
            return;
        }

        var levelSkeletonBiomsGeneratorParams = new LevelSkeletonBiomsGeneratorParams
        {
            LevelSkeleton = _levelSkeleton,
            MinOpenSpacePerimeter = levelSkeletonGeneratorParams.MinOpenSpacePerimeter,
            MaxOpenSpacePerimeter = levelSkeletonGeneratorParams.MaxOpenSpacePerimeter
        };

        var levelSkeletonBiomsGenerator = new LevelSkeletonBiomsGenerator(levelSkeletonBiomsGeneratorParams);

        _levelSkeletonWithBioms = levelSkeletonBiomsGenerator.Execute();

        if (_levelSkeletonWithBioms == null)
        {
            Debug.Log("[Failed] Bioms generation failed");
            return;
        }

        var levelGeneratorParams = new LevelGeneratorParams()
        {
            LevelSkeleton = _levelSkeletonWithBioms
        };

        var levelGenerator = new LevelGenerator(levelGeneratorParams);

        _level = levelGenerator.Execute();

        if (_level == null)
        {
            Debug.Log("[Failed] Level generation failed");
            return;
        }

        Redraw();
    }

    public void RegenerateBioms(float minEs, float maxEs)
    {
        _levelSkeletonWithBioms = null;
        _level = null;

        if (_levelSkeleton == null)
        {
            Debug.Log($"[Failed] Bioms generation failed: {nameof(_levelSkeleton)} is null");
            return;
        }

        var levelSkeletonBiomsGeneratorParams = new LevelSkeletonBiomsGeneratorParams()
        {
            LevelSkeleton = _levelSkeleton
        };

        if (minEs > 0)
            levelSkeletonBiomsGeneratorParams.MinOpenSpacePerimeter = minEs;

        if (maxEs > 0)
            levelSkeletonBiomsGeneratorParams.MaxOpenSpacePerimeter = maxEs;

        var levelSkeletonBiomsGenerator = new LevelSkeletonBiomsGenerator(levelSkeletonBiomsGeneratorParams);

        _levelSkeletonWithBioms = levelSkeletonBiomsGenerator.Execute();

        if (_levelSkeletonWithBioms == null)
        {
            Debug.Log("[Failed] Bioms generation failed");
            return;
        }

        var levelGeneratorParams = new LevelGeneratorParams()
        {
            LevelSkeleton = _levelSkeletonWithBioms
        };

        var levelGenerator = new LevelGenerator(levelGeneratorParams);

        _level = levelGenerator.Execute();

        if (_level == null)
        {
            Debug.Log("[Failed] Level generation failed");
            return;
        }

        Redraw();
    }

    public void RegenerateLevel()
    {
        _level = null;

        if (_levelSkeletonWithBioms == null)
        {
            Debug.Log($"[Failed] Level generation failed: {nameof(_levelSkeletonWithBioms)} is null");
            return;
        }

        var levelGeneratorParams = new LevelGeneratorParams()
        {
            LevelSkeleton = _levelSkeletonWithBioms
        };

        var levelGenerator = new LevelGenerator(levelGeneratorParams);

        _level = levelGenerator.Execute();

        if (_level == null)
        {
            Debug.Log("[Failed] Level generation failed");
            return;
        }

        Redraw();
    }

    private void Redraw()
    {
        if (ShowSkeleton.isOn && _levelSkeletonWithBioms != null)
        {
            _skeletonRenderer.Draw(_levelSkeletonWithBioms);
        }
        else if (ShowSkeleton.isOn && _levelSkeleton != null)
        {
            _skeletonRenderer.Draw(_levelSkeleton);
        }
        else
        {
            _skeletonRenderer.Clear();
        }

        if (ShowWalls.isOn && _level != null)
        {
            _levelRenderer.Draw(_level);

            _colliders?.ForEach(Destroy);
            _colliders = new List<GameObject>();

            foreach (var levelWall in _level.Walls)
            {
                _colliders.Add(levelWall.ToPlatform());
            }
        }
        else
        {
            _levelRenderer.Clear();
        }
    }
}
