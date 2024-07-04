using System;
using System.Collections;
using UnityEngine;

public class SteamMiscAchievements : MonoBehaviour
{
    [Header("Saving")]
    [SerializeField] private float timeBetweenAutoSaves = 5f;

    // time save
    private int timeSpentIngame = 0;

    //coroutines
    private bool running = true;

    private void Start()
    {
        // get spent time
        timeSpentIngame = MySteamAchievementHandler.GetStatProgress(SteamStatNames.STAT_MINUTES_SPENT);
        StartCoroutine(MinuteTick());

        // start autosave routine
        StartCoroutine(SaveTick());

        CheckIfPlayedOnRelease();

        // disable this after game has been released
        CheckIfTutorialCompleted();
    }

    private void OnDisable()
    {
        running = false;
    }

    private void CheckIfPlayedOnRelease()
    {
        // check
        var achievements = Enum.GetNames(typeof(SteamAchNames));
        for (int i = 0; i < achievements.Length; i++)
        {
            Steamworks.SteamUserStats.GetAchievementAndUnlockTime(achievements[i], out bool unlocked, out uint epochTime);
            if (!unlocked) continue;

                //print(epochTime);
            if (1671541200/*startdate*/ < epochTime && epochTime < 1672146000/*enddate*/)
            {
                MySteamAchievementHandler.SetAchievementCompleted(SteamAchNames.ACHIEVEMENT_PLAY_ON_RELEASE);
                return;
            }
        }
    }

    private void CheckIfTutorialCompleted()
    {
        if (GameSaves.AskFor.IsLevelComplete("Tutorial"))
        {
            MySteamAchievementHandler.SetAchievementCompleted(SteamAchNames.ACHIEVEMENT_COMPLETED_TUTORIAL);
        }
    }


    private void CheckHoursPlayed()
    {
        MySteamAchievementHandler.SaveAchievementProgress(timeSpentIngame, SteamStatNames.STAT_MINUTES_SPENT, SteamAchNames.ACHIEVEMENT_HOURS_PLAYED_2);
    }

    private IEnumerator MinuteTick()
    {
        while (running)
        {
            // we wait 62 seconds per minute just in case unity counts faster than steam. We want to be behind steam rather than in front of steam.
            yield return new WaitForSecondsRealtime(62f);
            timeSpentIngame++;
            CheckHoursPlayed();
            //print("checked minutes played");
        }
    }

    private IEnumerator SaveTick()
    {
        while (running)
        {
            yield return new WaitForSecondsRealtime(timeBetweenAutoSaves);
            MySteamAchievementHandler.SaveProgress();
            //print("save tick");
        }
    }
}
