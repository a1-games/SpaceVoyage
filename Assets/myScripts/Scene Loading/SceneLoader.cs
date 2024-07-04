using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string[] stageNames =
    {
        // These names are the folders that are searched to generate the level choices
        "DEMO",
        "EXPO",
        "Stage1",
        "Stage2",
        "Stage3",
        "Stage4",
        "Stage5",
        "Stage6",
    };

    private static SceneLoader instance;
    public static SceneLoader AskFor { get => instance; }
    public bool isDemo = false;

    public List<Scene> scenes;
    public GameObject loadingOverlay;
    public TMP_Text loadingText;
    public List<Camera> camerasToDisableOnLoading;

    private Coroutine loadingCR;
    private void Awake()
    {
        instance = this;
        loadingOverlay.SetActive(false);
    }
    private void OnDisable()
    {
        if (loadingCR != null)
            StopCoroutine(loadingCR);
    }
    public void ChangeScene(string sceneName)
    {
        GameSettings.AskFor.SaveAll();
        Time.timeScale = 1f;
        //print("coroutine started");
        loadingCR = StartCoroutine(LoadSceneAsyncWithInfo(sceneName));
    }

    IEnumerator LoadSceneAsyncWithInfo(string sceneName)
    {
        if (camerasToDisableOnLoading != null)
        {
            for (int i = 0; i < camerasToDisableOnLoading.Count; i++)
            {
                if (camerasToDisableOnLoading[i])
                    camerasToDisableOnLoading[i].enabled = false;
            }
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        operation.allowSceneActivation = true;
        loadingOverlay.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            float percent = progress * 100f;
            string rounded = Mathf.Round(percent).ToString();

            //print(rounded);
            loadingText.text = "Loading\n" + rounded + "% completed";


            yield return null;
        }
        loadingOverlay.SetActive(false);
    }

    public void LoadTutorialIfNeverPlayed()
    {
        if (GameSettings.AskFor.GetTutorialCompleted()) return;
        //print("tutorial has been completed ?: " + GameSettings.AskFor.GetTutorialCompleted());
        ChangeScene("Tutorial");
    }

    // Automatic Scene Switching
    public string GetCurrentLevelTitle()
    {
        string numberString = SceneNameToNumbers(SceneManager.GetActiveScene().name).ToString();
        char[] allNumbers = new char[10] { '0','1', '2', '3', '4', '5', '6', '7', '8', '9' };

        var charIndex = numberString.IndexOfAny(allNumbers); //get the index of the first number in the string
        var stageNr = numberString.Substring(0, charIndex + 1); // get the first number only
        var levelNr = numberString.Substring(charIndex + 1); // get every number after the first number
        int lvlNrInt = Int32.Parse(levelNr);
        if (lvlNrInt <= 9)
            levelNr = levelNr.Replace("0", ""); //remove 0 from level nr

        return "Stage " + stageNr + " Level " + levelNr;
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        GameSaves.AskFor.SaveAll();
        GameSettings.AskFor.SaveAll();
        if (!CanLoadNextLevel()) return;

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName.Contains("utorial"))// no 'T' so we can ignore capitalization // if this button is pressed in the tutorialscene
        {
            var scenes = SceneNames();
            var loadableScenes = new List<string>();
            for (int i = 0; i < scenes.Length; i++)
            {
                // skip item if not loadable
                if (scenes[i].Contains("Menu") || scenes[i].Contains("torial")) continue;
                loadableScenes.Add(scenes[i]);

                // this will always give the lowest numbered level, so are done once we have the first item
                if (loadableScenes.Count >= 1) break;
            }

            ChangeScene(loadableScenes[0]);
            return;
        }

        // get the scene number from current scene
        int currentSceneNr = SceneNameToNumbers(currentSceneName);
        // get stage names so it doesnt get called for each if statement in the loop - only for performance optimization reasons
        var stageNames = StageNames();


        for (int i = 0; i < stageNames.Length; i++)
        {
            var sceneNames = SceneNamesFromStage(stageNames[i]);

            for (int j = 0; j < sceneNames.Length; j++)
            {
                var nextNr = SceneNameToNumbers(sceneNames[j]);

                if (nextNr == currentSceneNr + 1)
                {
                    ChangeScene(sceneNames[j]); //load scene with the full sceneName
                    return;
                }
                // if we are in the last level of the current stage and the current stage is the current scene's stage
                if (j == sceneNames.Length - 1 && currentSceneName.Contains(stageNames[i]) && CanLoadNextLevel())
                {
                    //this should load the first level of the next stage - EXAMPLE: called in stage1 last level, start in stage2 first level
                    ChangeScene(SceneNamesFromStage(stageNames[i/* i should be the stage nr of active scene*/ + 1])[0]); 
                    return;
                }
            }
        }

        Debug.LogError($"Could not load next level from {currentSceneName}/{currentSceneNr} | Could not find {currentSceneNr + 1} in {stageNames}. Perhaps Stage Name is not included in Sceneloader.StageNames() or Scene is not included in Build Settings ?");
    }
    
    public bool CanLoadNextLevel() // returns true if there is a scene with a higher number than the current
    {
        var sceneNames = SceneNames();

        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneNames[i].Contains("Level"))
            {
                int levelNr = SceneNameToNumbers(sceneNames[i]); // get comparing scene number

                if (levelNr > GetCurrentLevelNumber()) // if at least one level comes after the current scene
                {
                    return true;
                }
            }
        }
        //if it doesnt go in the loop and return true it will default to false
        return false;
    }

    public void ReloadThisScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        ChangeScene(sceneName);
    }

    // complicated scene name stuff

    public int GetCurrentLevelNumber()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // if this button is pressed in the tutorialscene
        if (!currentSceneName.Contains("Level"))
        {
            return 0;
        }

        // get number from scene
        return SceneNameToNumbers(currentSceneName);
    }

    public static int SceneNameToNumbers(string sceneName)
    {
        string extractedSceneNumber = "";
        // convert stage names to only numbers - EXAMPLE: Stage1Level04 will be 104
        for (int j = 0; j < sceneName.Length; j++)
        {
            if (Char.IsDigit(sceneName[j]))
                extractedSceneNumber += sceneName[j];
        }
        // convert to int and return it
        if (extractedSceneNumber != "") return int.Parse(extractedSceneNumber);
        return 0;
    }
    public string[] SceneNamesFromStage(string stageName)
    {
        var sceneNames = SceneNames();

        List<string> newNames = new List<string>();

        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneNames[i].Contains(stageName))
            {
                newNames.Add(sceneNames[i]);
                continue;
            }
        }
        return newNames.ToArray();
    }

    public string[] LevelNames(string stageName)
    {
        var sceneNames = SceneNames();

        List<string> levelNames = new List<string>();

        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneNames[i].Contains("Level"))
            {
                if (!sceneNames[i].Contains(stageName)) continue; // i used break; here and spent an hour looking for my mistake....

                var tmpName = sceneNames[i];
                var substringIndex = tmpName.LastIndexOf("L");
                tmpName = tmpName.Substring(substringIndex);
                levelNames.Add(tmpName);
            }
        }
        return levelNames.ToArray();
    }

    public string[] StageNames()
    {
        return stageNames;
    }

    public string[] SceneNames() // ADD A SORTING MECHANISM - if the levels aren't sorted the level tile spawner placed them in incorrect order
    {
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
        string[] scenes = new string[sceneCount];
        for (int i = 0; i < sceneCount; i++)
        {
            scenes[i] = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
        }
        // sort the scenenames alphabetically and numerically, just in case some scenes are not in order in the build settings
        Array.Sort(scenes); 
        return scenes;
    }


}
