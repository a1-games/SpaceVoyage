using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageSelectUIItem : MonoBehaviour
{
    [SerializeField] private TMP_Text levelAmount_UI;
    [SerializeField] private TMP_Text stageName_UI;
    [SerializeField] private TMP_Text completionPercentage_UI;
    [SerializeField] private Slider completionSlider;

    public string StageName { get; set; }
    public string LevelAmount { get; set; }
    public float completionPercentage { get; private set; }

    public void SetVariables()
    {
        StageName = stageName_UI.text;

        var levelNameInput = StageName.Replace(" ", "");
        LevelAmount = SceneLoader.AskFor.LevelNames(levelNameInput).Length + " Levels"; 

        levelAmount_UI.text = LevelAmount;

        SetCompletionPercentage();
        SetCompletionSlider(completionPercentage);
    }
    private void SetCompletionSlider(float percentage)
    {
        completionPercentage_UI.text = percentage + "% Completed";
        // i have set the slider to values 0 to 100. if not set to this, it will always be full bc 1 will be 100%
        completionSlider.value = percentage; 
    }
    
    public void SetCompletionPercentage()
    {
        var completedlevels = 0;
        var stageName = StageName.Replace(" ", "");
        var levelNames = SceneLoader.AskFor.SceneNamesFromStage(stageName);
        //print(stageName);
        for (int i = 0; i < levelNames.Length; i++)
        {
            if (GameSaves.AskFor.IsLevelComplete(levelNames[i]))
            {
                completedlevels++;
                //print(completedlevels);
            }
        }

        // no reason for math if 0 levels completed
        if (completedlevels == 0)
        {
            completionPercentage = 0f;
            return;
        }

        //math
        float perc = (completedlevels / (float)levelNames.Length) * 100f;
        perc = Mathf.Round(perc);

        completionPercentage = perc;// * 100 bc its a decimal value into a percentage
    }


    // i didn't need this but it is good code so i don't want to delete it
    /*
    [SerializeField] private GameObject[] stars;
    public void SetStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }
        for (int i = 0; i < CountingStars(); i++)
        {
            stars[i].SetActive(true);
        }
    }
    public int CountingStars()
    {
        var completedlevels = 0;
        var stageName = StageName.Replace(" ", "");
        var levelNames = SceneLoader.AskFor.SceneNamesFromStage(stageName);
        //print(stageName);
        foreach (string name in levelNames)
        {
            //print(name);
            if (GameSaves.AskFor.IsLevelComplete(name))
            {
                completedlevels++;
                //print(completedlevels);
            }
        }

        if (completedlevels == 0) return 0;
        //print((float)completedlevels / (float)levelNames.Length);
        return Mathf.FloorToInt(((float)completedlevels / (float)levelNames.Length) * stars.Length); // lesson learned, never calculate with ints
    }
    */
}
