using UnityEngine;
using UnityEngine.UI;

public class UiControllerScript : MonoBehaviour
{
    public InputField AdditionalLines;
    public Toggle ReplaceMoreThan45Degree;
    public Toggle MergeNearBy;

    public InputField MinES;
    public InputField MaxES;

    public Toggle RegenerateSkeleton;
    public Toggle RegenerateSkeletonBioms;
    public Toggle RegenerateLevel;

    public Button GenerateButton;
    public GameObject LevelGeneratorController;

    void Start()
    {
        GenerateButton.onClick.AddListener(GenerateOnClick);
    }

    void GenerateOnClick()
    {

        if(RegenerateSkeleton.isOn)
        {
            var levelSkeletonGeneratorParams = new LevelSkeletonGeneratorParams();
            levelSkeletonGeneratorParams.ReplaceMoreThan45 = ReplaceMoreThan45Degree.isOn;
            levelSkeletonGeneratorParams.MergeNearBy = MergeNearBy.isOn;

            var minES = 0f;
            float.TryParse(MinES.text, out minES);

            var maxES = 0f;
            float.TryParse(MaxES.text, out maxES);

            if (minES > 0f)
                levelSkeletonGeneratorParams.MinOpenSpacePerimeter = minES;

            if (maxES > 0f)
                levelSkeletonGeneratorParams.MaxOpenSpacePerimeter = maxES;

            if (int.TryParse(AdditionalLines.text, out int additionalLines))
            {
                levelSkeletonGeneratorParams.AdditionalLinesCount = additionalLines;
            }

            LevelGeneratorController.GetComponent<LevelGeneratorController>().Regenerate(levelSkeletonGeneratorParams);
        } else if(RegenerateSkeletonBioms.isOn)
        {
            var minES = 0f;
            float.TryParse(MinES.text, out minES);

            var maxES = 0f;
            float.TryParse(MaxES.text, out maxES);

            LevelGeneratorController.GetComponent<LevelGeneratorController>().RegenerateBioms(minES, maxES);
        } else if (RegenerateLevel.isOn)
        {
            LevelGeneratorController.GetComponent<LevelGeneratorController>().RegenerateLevel();
        }
        
    }
}
