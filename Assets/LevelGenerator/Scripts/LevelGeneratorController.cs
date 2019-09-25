using UnityEngine;
using UnityEngine.UI;

public class LevelGeneratorController : MonoBehaviour
{
    private LevelSkeleton _levelSkeleton;
    private Level _level;
    private LevelSkeletonRenderer _skeletonRenderer;
    private LevelRenderer _levelRenderer;

    public Toggle ShowSkeleton;
    public Toggle ShowWalls;

    // Start is called before the first frame update
    void Start()
    {
        var levelSkeletonGeneratorParams = new LevelSkeletonGeneratorParams();
        Generate(levelSkeletonGeneratorParams);

        Init();
        ShowSkeleton.onValueChanged.AddListener(delegate {
            Redraw();
        });
        ShowWalls.onValueChanged.AddListener(delegate {
            Redraw();
        });
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Generate(LevelSkeletonGeneratorParams levelSkeletonGeneratorParams)
    {
        Init();
        var levelSkeletonGenerator = new LevelSkeletonGenerator(levelSkeletonGeneratorParams);

        _levelSkeleton = levelSkeletonGenerator.Execute();

        if (_levelSkeleton == null)
        {
            Debug.Log("[Failed] generation failed");
            return;
        }

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

        Redraw();
    }

    void Redraw()
    {
        if (ShowSkeleton.isOn && _levelSkeleton != null)
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
        }
        else
        {
            _levelRenderer.Clear();
        }
    }
}
