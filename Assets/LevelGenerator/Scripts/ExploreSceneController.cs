using Assets.PC.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(LevelRenderer))]
public class ExploreSceneController : MonoBehaviour
{
    private LevelRenderer _levelRenderer;
    public Button BackButton;
    public GameObject ElevatorPrefab;
    public GameObject Player;

    void Start()
    {
        _levelRenderer = GetComponent<LevelRenderer>();
        BackButton.onClick.AddListener(LoadLevelGeneratorScene);

        if (LevelHolder.Level == null)
        {
            Debug.Log($"[Failed] Failed to load level: {nameof(LevelHolder.Level)} is null");
            return;
        }

        // Draw walls
        _levelRenderer.Draw(LevelHolder.Level);
        
        //// Create wall colliders
        //foreach (var levelWall in LevelHolder.Level.Walls)
        //{
        //    levelWall.ToPlatform();
        //}

        foreach (var levelElevator in LevelHolder.Level.Elevators)
        {
            levelElevator.ToElevator(ElevatorPrefab);
        }

        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (LevelHolder.PortalIndex == null)
            return;

        var portals = GameObject.FindGameObjectsWithTag("Respawn");

        foreach (var portal in portals)
        {
            var doorToSceneController = portal.GetComponent<DoorToSceneController>();

            if (doorToSceneController == null)
                continue;

            if (doorToSceneController.Index == LevelHolder.PortalIndex)
            {
                Player.transform.position = portal.transform.position;
                LevelHolder.PortalIndex = null;
            }
        }
    }

    private void LoadLevelGeneratorScene()
    {
        SceneManager.LoadScene("LevelGenerator", LoadSceneMode.Single);
    }
}
