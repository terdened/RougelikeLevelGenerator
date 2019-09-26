using UnityEngine;
using UnityEngine.UI;

public class UiControllerScript : MonoBehaviour
{
    public InputField AdditionalLines;
    public Toggle ReplaceMoreThan45Degree;
    public Toggle MergeNearBy;
    
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

            if (int.TryParse(AdditionalLines.text, out int additionalLines))
            {
                levelSkeletonGeneratorParams.AdditionalLinesCount = additionalLines;
            }

            LevelGeneratorController.GetComponent<LevelGeneratorController>().Regenerate(levelSkeletonGeneratorParams);
        } else if(RegenerateSkeletonBioms.isOn)
        {
            LevelGeneratorController.GetComponent<LevelGeneratorController>().RegenerateBioms();
        } else if (RegenerateLevel.isOn)
        {
            LevelGeneratorController.GetComponent<LevelGeneratorController>().RegenerateLevel();
        }
        
    }
}
