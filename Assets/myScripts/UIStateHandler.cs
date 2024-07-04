using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum UIState
{
    TitleScreen,
    MainMenu,
    StageSelect,
    Settings,
    GameLost,
    GameWon,
    Paused,
    InGame,
    LevelSelect,
}

public class UIStateHandler : MonoBehaviour
{
    private static UIStateHandler instance;
    public static UIStateHandler AskFor { get => instance; }

    public static UIState CurrentUIState { get; set; }
    public UIState LastUIState { get; private set; }

    public static bool HasSeenTitleScreen { get; set; }

    public PanelUI[] panels;

    private void Awake()
    {
        instance = this;

        if (!HasSeenTitleScreen && SceneManager.GetActiveScene().name == "MainMenuScene") CurrentUIState = UIState.TitleScreen;
        RefreshUI(CurrentUIState);
    }

    public void RefreshUI(string state)
    {
        if (FixScrollRect.isDragging) return;

        LastUIState = CurrentUIState;

        switch (state)
        {
            case "TitleScreen":
                {
                    CurrentUIState = UIState.TitleScreen;
                    break;
                }
            case "MainMenu":
                {
                    CurrentUIState = UIState.MainMenu;
                    break;
                }
            case "StageSelect":
                {
                    CurrentUIState = UIState.StageSelect;
                    break;
                }
            case "LevelSelect":
                {
                    CurrentUIState = UIState.LevelSelect;
                    break;
                }
            case "Settings":
                {
                    CurrentUIState = UIState.Settings;
                    break;
                }
            case "InGame":
                {
                    CurrentUIState = UIState.InGame;
                    break;
                }
            case "GameLost":
                {
                    CurrentUIState = UIState.GameLost;
                    break;
                }
            case "GameWon":
                {
                    CurrentUIState = UIState.GameWon;
                    break;
                }
            case "Paused":
                {
                    CurrentUIState = UIState.Paused;
                    break;
                }
        }

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].RefreshUI(CurrentUIState);
        }
    }
    public void GoToPreviousUIState()
    {
        RefreshUI(LastUIState);
    }
    public void CloseTitleScreen()
    {
        HasSeenTitleScreen = true;
        RefreshUI(UIState.MainMenu);
    }
    public void RefreshUI(UIState state)
    {
        LastUIState = CurrentUIState;
        CurrentUIState = state;

        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].RefreshUI(CurrentUIState);
        }
    }


}
