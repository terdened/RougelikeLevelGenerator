using UnityEngine;
using UnityEngine.UI;

public class UiControllerScript : MonoBehaviour
{
    public InputField AdditionalLines;
    public Toggle ReplaceMoreThan45Degree;
    public Toggle MergeNearBy;
    public Button GenerateButton;
    public GameObject LevelGeneratorController;

    void Start()
    {
        GenerateButton.onClick.AddListener(GenerateOnClick);
    }

    void GenerateOnClick()
    {
        var levelSkeletonGeneratorParams = new LevelSkeletonGeneratorParams();
        levelSkeletonGeneratorParams.ReplaceMoreThan45 = ReplaceMoreThan45Degree.isOn;
        levelSkeletonGeneratorParams.MergeNearBy = MergeNearBy.isOn;

        if (int.TryParse(AdditionalLines.text, out int additionalLines))
        {
            levelSkeletonGeneratorParams.AdditionalLinesCount = additionalLines;
        }

        LevelGeneratorController.GetComponent<LevelGeneratorController>().Generate(levelSkeletonGeneratorParams);
    }
}
