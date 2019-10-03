﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(LevelRenderer))]
public class ExploreSceneController : MonoBehaviour
{
    private LevelRenderer _levelRenderer;
    public Button BackButton;

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
        _levelRenderer.Draw(LevelHolder.Level, 0.005f);
        
        // Create wall colliders
        foreach (var levelWall in LevelHolder.Level.Walls)
        {
            levelWall.ToPlatform();
        }
    }

    private void LoadLevelGeneratorScene()
    {
        SceneManager.LoadScene("LevelGenerator", LoadSceneMode.Single);
    }
}