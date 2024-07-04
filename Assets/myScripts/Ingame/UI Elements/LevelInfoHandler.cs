using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfoHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text stageNumberEndScreen;
    [SerializeField] private TMPro.TMP_Text stageNumberPauseScreen;
    [SerializeField] private TMPro.TMP_Text levelNameTopRight;
    [SerializeField] private GameObject nextLevelButton;

    [Header("DEMO ?")]
    [SerializeField] private bool isDemoBuild = false;

    private void Start()
    {
        if (!isDemoBuild)
        {
            string stageName = SceneLoader.AskFor.GetCurrentLevelTitle();

            stageNumberEndScreen.text = stageName;
            stageNumberPauseScreen.text = stageName;
            levelNameTopRight.text = stageName;
        }

        if (!SceneLoader.AskFor.CanLoadNextLevel()) nextLevelButton.SetActive(false);
    }
}
