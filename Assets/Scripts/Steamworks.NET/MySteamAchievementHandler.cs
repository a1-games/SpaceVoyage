using UnityEngine;
using Steamworks;

public enum SteamAchNames
{
    ACHIEVEMENT_COMPLETED_TUTORIAL, // Set
    ACHIEVEMENT_LEVELS_COMPLETED_3, // Set
    ACHIEVEMENT_LEVELS_COMPLETED_20, // Set
    ACHIEVEMENT_LEVELS_COMPLETED_ALL, // Set
    ACHIEVEMENT_BARRELS_MOVED_250, // Set
    ACHIEVEMENT_MISSILES_FIRED_100, // Set
    ACHIEVEMENT_HOURS_PLAYED_2, // Set
    ACHIEVEMENT_PLAY_ON_RELEASE,
    ACHIEVEMENT_HACKERMAN, // Set
}
public enum SteamStatNames
{
    STAT_BARRELS_MOVED,
    STAT_MISSILES_FIRED,
    STAT_LEVELS_COMPLETED,
    STAT_MINUTES_SPENT,
}
public class MySteamAchievementHandler : MonoBehaviour
{
    private MySteamAchievementHandler instance;
    public MySteamAchievementHandler AskFor { get => instance; }
    private void Awake()
    {
        instance = this;
        //if (SteamManager.Initialized)
           // Initialized = true;
    }

    //public static bool Initialized { get; private set; } = false;
    public static bool Initialized 
    { 
        get => SteamManager.Initialized; 
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }
    private void OnDisable()
    {
        //SaveProgress();
    }
    public static void SaveProgress()
    {
        if (!Initialized) return;
        SteamUserStats.StoreStats();
    }
    public static void ResetSteamStats()
    {
        if (!Initialized) return;
        SteamUserStats.ResetAllStats(true);
    }
    public static void SetAchievementCompleted(SteamAchNames achievementName)
    {
        if (!Initialized) return;
        SteamUserStats.SetAchievement(achievementName.ToString());
    }
    public static int GetStatProgress(SteamStatNames statName)
    {
        if (!Initialized) return 0;
        SteamUserStats.GetStat(statName.ToString(), out int progress);
        return progress;
    }
    public static void SetStatProgress(SteamStatNames statName, int progress)
    {
        if (!Initialized) return;
        SteamUserStats.SetStat(statName.ToString(), progress);
    }

    public static void SaveAchievementProgress(int localProgress, SteamStatNames statName, SteamAchNames achievementName)
    {
        if (!Initialized) return;
        // get steam progress
        var steamProgress = GetStatProgress(statName);
        //print(statName + " progress is " + steamProgress);
        // set steam progress if local progress is higher
        if (steamProgress < localProgress)
            SetStatProgress(statName, localProgress);
        // unlock the achievement if the progress is higher than unlock criteria
        UnlockAchievementWithStatCheck(statName, achievementName);
    }
    public static void SaveMultipleAchievementProgress(int localProgress, SteamStatNames statName, SteamAchNames[] achievementNames)
    {
        if (!Initialized) return;
        // get steam progress
        var steamProgress = GetStatProgress(statName);
        // set steam progress if local progress is higher
        if (steamProgress < localProgress)
            SetStatProgress(statName, localProgress);
        // unlock the achievement if the progress is higher than unlock criteria
        for (int i = 0; i < achievementNames.Length; i++)
        {
            UnlockAchievementWithStatCheck(statName, achievementNames[i]);
        }
    }

    // returns true if the checked stat has been 100% completed
    private static void UnlockAchievementWithStatCheck(SteamStatNames statName, SteamAchNames achievementName)
    {
        if (!Initialized) return;
        SteamUserStats.GetStat(statName.ToString(), out int progress);
        SteamUserStats.GetAchievementProgressLimits(achievementName.ToString(), out int min, out int max);
        //print("achievement limit: " + max);
        if (progress >= max)
            SetAchievementCompleted(achievementName);
    }

}
