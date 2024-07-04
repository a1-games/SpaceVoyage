using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaves : MonoBehaviour
{
    private static GameSaves instance;
    public static GameSaves AskFor { get => instance; }
    private void Awake()
    {
        instance = this;
        //print(Application.dataPath.ToString());
    }

    private SteamAchNames[] levelsCompletedAchievements = new SteamAchNames[]
    {
        SteamAchNames.ACHIEVEMENT_LEVELS_COMPLETED_3,
        SteamAchNames.ACHIEVEMENT_LEVELS_COMPLETED_20,
        SteamAchNames.ACHIEVEMENT_LEVELS_COMPLETED_ALL,
    };

    public void SaveAll()
    {
        GameSettings.jPlayerPrefs.Save();
    }
    public void SaveLevelComplete(string StageXLevelXX)
    {
        //print("saved level " + "LEVEL_COMPLETE_" + StageXLevelXX);
        GameSettings.jPlayerPrefs.SetInt("LEVEL_COMPLETE_" + StageXLevelXX, 1);
        //print("saved level" + StageXLevelXX);
        GameSettings.jPlayerPrefs.Save();

        if (StageXLevelXX.Contains("Tut"))
        {
            MySteamAchievementHandler.SetAchievementCompleted(SteamAchNames.ACHIEVEMENT_COMPLETED_TUTORIAL);
        }
        else
        {

            MySteamAchievementHandler.SaveMultipleAchievementProgress(GetLevelsCompletedCount(),
                                                                       SteamStatNames.STAT_LEVELS_COMPLETED,
                                                                       levelsCompletedAchievements);
        }
    }

    public bool IsLevelComplete(string StageXLevelXX)
    {
        // returns true if stagelevel is not 0
        return GameSettings.jPlayerPrefs.GetInt("LEVEL_COMPLETE_" + StageXLevelXX, 0) == 0 ? false : true; 
    }

    public int GetLevelsCompletedCount()
    {
        var sceneNames = SceneLoader.AskFor.SceneNames();
        int count = 0;

        for (int i = 0; i < sceneNames.Length; i++)
        {
            // skip if it isn't a level
            if (!sceneNames[i].ToLower().Contains("level")) continue;

            if (IsLevelComplete(sceneNames[i]))
            {
                count++;
            }
        }

        return count;
    }
}
