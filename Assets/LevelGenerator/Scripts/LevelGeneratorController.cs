using UnityEngine;
using UnityEngine.UI;

public class LevelGeneratorController : MonoBehaviour
{
    private LevelSkeletonRenderer _skeletonRenderer;
    private LevelMapRenderer _levelMapRenderer;

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


        if (LevelHolder.Level == null)
        {
            var levelSkeletonGeneratorParams = new LevelSkeletonGeneratorParams();
            Regenerate(levelSkeletonGeneratorParams);
        }
        else
        {
            Redraw();
        }
    }

    void Init()
    {
        if(_skeletonRenderer == null)
        {
            _skeletonRenderer = GetComponent<LevelSkeletonRenderer>();
        }

        if (_levelMapRenderer == null)
        {
            _levelMapRenderer = GetComponent<LevelMapRenderer>();
        }
    }

    public void Regenerate(LevelSkeletonGeneratorParams levelSkeletonGeneratorParams)
    {
        LevelHolder.LevelSkeleton = null;
        LevelHolder.LevelSkeletonWithBioms = null;
        LevelHolder.Level = null;

        var levelSkeletonGenerator = new LevelSkeletonGenerator(levelSkeletonGeneratorParams);

        LevelHolder.LevelSkeleton = levelSkeletonGenerator.Execute();

        if (LevelHolder.LevelSkeleton == null)
        {
            Debug.Log("[Failed] generation failed");
            return;
        }

        var levelSkeletonBiomsGeneratorParams = new LevelSkeletonBiomsGeneratorParams
        {
            LevelSkeleton = LevelHolder.LevelSkeleton,
            MinOpenSpacePerimeter = levelSkeletonGeneratorParams.MinOpenSpacePerimeter,
            MaxOpenSpacePerimeter = levelSkeletonGeneratorParams.MaxOpenSpacePerimeter
        };

        var levelSkeletonBiomsGenerator = new LevelSkeletonBiomsGenerator(levelSkeletonBiomsGeneratorParams);

        LevelHolder.LevelSkeletonWithBioms = levelSkeletonBiomsGenerator.Execute();

        if (LevelHolder.LevelSkeletonWithBioms == null)
        {
            Debug.Log("[Failed] Bioms generation failed");
            return;
        }

        var levelGeneratorParams = new LevelGeneratorParams()
        {
            LevelSkeleton = LevelHolder.LevelSkeletonWithBioms
        };

        var levelGenerator = new LevelGenerator(levelGeneratorParams);

        LevelHolder.Level = levelGenerator.Execute();

        if (LevelHolder.Level == null)
        {
            Debug.Log("[Failed] Level generation failed");
            return;
        }

        Redraw();
    }

    public void RegenerateBioms(float minEs, float maxEs)
    {
        LevelHolder.LevelSkeletonWithBioms = null;
        LevelHolder.Level = null;

        if (LevelHolder.LevelSkeleton == null)
        {
            Debug.Log($"[Failed] Bioms generation failed: {nameof(LevelHolder.LevelSkeleton)} is null");
            return;
        }

        var levelSkeletonBiomsGeneratorParams = new LevelSkeletonBiomsGeneratorParams()
        {
            LevelSkeleton = LevelHolder.LevelSkeleton
        };

        if (minEs > 0)
            levelSkeletonBiomsGeneratorParams.MinOpenSpacePerimeter = minEs;

        if (maxEs > 0)
            levelSkeletonBiomsGeneratorParams.MaxOpenSpacePerimeter = maxEs;

        var levelSkeletonBiomsGenerator = new LevelSkeletonBiomsGenerator(levelSkeletonBiomsGeneratorParams);

        LevelHolder.LevelSkeletonWithBioms = levelSkeletonBiomsGenerator.Execute();

        if (LevelHolder.LevelSkeletonWithBioms == null)
        {
            Debug.Log("[Failed] Bioms generation failed");
            return;
        }

        var levelGeneratorParams = new LevelGeneratorParams()
        {
            LevelSkeleton = LevelHolder.LevelSkeletonWithBioms
        };

        var levelGenerator = new LevelGenerator(levelGeneratorParams);

        LevelHolder.Level = levelGenerator.Execute();

        if (LevelHolder.Level == null)
        {
            Debug.Log("[Failed] Level generation failed");
            return;
        }

        Redraw();
    }

    public void RegenerateLevel()
    {
        LevelHolder.Level = null;

        if (LevelHolder.LevelSkeletonWithBioms == null)
        {
            Debug.Log($"[Failed] Level generation failed: {nameof(LevelHolder.LevelSkeletonWithBioms)} is null");
            return;
        }

        var levelGeneratorParams = new LevelGeneratorParams()
        {
            LevelSkeleton = LevelHolder.LevelSkeletonWithBioms
        };

        var levelGenerator = new LevelGenerator(levelGeneratorParams);

        LevelHolder.Level = levelGenerator.Execute();

        if (LevelHolder.Level == null)
        {
            Debug.Log("[Failed] Level generation failed");
            return;
        }

        Redraw();
    }

    private void Redraw()
    {
        if (ShowSkeleton.isOn && LevelHolder.LevelSkeletonWithBioms != null)
        {
            _skeletonRenderer.Draw(LevelHolder.LevelSkeletonWithBioms);
        }
        else if (ShowSkeleton.isOn && LevelHolder.LevelSkeleton != null)
        {
            _skeletonRenderer.Draw(LevelHolder.LevelSkeleton);
        }
        else
        {
            _skeletonRenderer.Clear();
        }

        if (ShowWalls.isOn && LevelHolder.Level != null)
        {
            _levelMapRenderer.Draw(LevelHolder.Level);
        }
        else
        {
            _levelMapRenderer.Clear();
        }
    }
}
