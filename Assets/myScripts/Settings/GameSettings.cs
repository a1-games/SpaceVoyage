using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Steamworks;

public class GameSettings : MonoBehaviour
{
    private static GameSettings instance;
    public static GameSettings AskFor { get => instance; }
    
    public float MasterVolume { get => Slider_Master.value; }
    public float MusicVolume { get => Slider_Music.value; }

    [SerializeField] private GameObject confirmationPopUpObject;
    private ConfirmationPopUp confirmationPopUp;
    [SerializeField] private GameObject keybindsList;

    [Header("Avanced")]
    [SerializeField] private ResolutionSettings resolutionSettings;
    [SerializeField] private GameObject advancedSettings;
    [Header("Sounds")]
    [SerializeField] private UISounds uiSounds;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Slider Slider_Music;
    [SerializeField] private Slider Slider_Master;

    //[SerializeField] private Button settingsButton;
    [Header("Toggles")]
    [SerializeField] private Button[] exitButtons;
    [SerializeField] private Toggle colourblindToggle;
    [SerializeField] private Toggle camRotationToggle;
    [SerializeField] private Toggle camSwitchOnStartToggle;
    [SerializeField] private Toggle maxGameSpeedToggle;
    [SerializeField] private ToggleGroup qualityToggleGroup;
    public List<ColourblindAssistObject> CB_Assists { get; set; }

    private static JsonPlayerPrefs _jplayerPrefs;
    public static JsonPlayerPrefs jPlayerPrefs 
    { 
        get 
        { 
            if (_jplayerPrefs == null) _jplayerPrefs = new JsonPlayerPrefs(Application.dataPath + "/saves/Preferences.json");
            return _jplayerPrefs;
        }
    }
    private void Awake()
    {
        instance = this;
        
        //---
        for (int i = 0; i < exitButtons.Length; i++)
        {
            exitButtons[i].onClick.RemoveAllListeners();
            exitButtons[i].onClick.AddListener( () => jPlayerPrefs.Save() );
            exitButtons[i].onClick.AddListener( () => uiSounds.Exit() );
            exitButtons[i].onClick.AddListener( () => UIStateHandler.AskFor.GoToPreviousUIState() );
        }

        // GAME SOUNDS
        Slider_Master.value = jPlayerPrefs.GetFloat("SETTINGS_VOLUME_MASTER", 0.5f);
        //does not cause sound at first frame
        ChangeVolumeMaster();
        
        Slider_Music.value = jPlayerPrefs.GetFloat("SETTINGS_VOLUME_MUSIC", 0.5f);
        //does not cause sound at first frame
        ChangeVolumeMusic();

        // PLAYER CAMERA ROTATION
        //does not cause sound at first frame bc we add sound after changing values
        camRotationToggle.isOn = GetRotatePlayerCamWithPlayer();
        camRotationToggle.onValueChanged.AddListener(delegate { uiSounds.Click(); });
        camRotationToggle.onValueChanged.AddListener(delegate { jPlayerPrefs.Save(); });
        // CAMERA AUTO SWITCH ON START
        //does not cause sound at first frame bc we add sound after changing values
        camSwitchOnStartToggle.isOn = GetCameraAutoSwitchOnStart();
        camSwitchOnStartToggle.onValueChanged.AddListener(delegate { uiSounds.Click(); });
        camSwitchOnStartToggle.onValueChanged.AddListener(delegate { jPlayerPrefs.Save(); });
        // CAMERA AUTO SWITCH ON START
        //does not cause sound at first frame bc we add sound after changing values
        maxGameSpeedToggle.isOn = GetAlwaysMaxGameSpeed();
        maxGameSpeedToggle.onValueChanged.AddListener(delegate { uiSounds.Click(); });
        maxGameSpeedToggle.onValueChanged.AddListener(delegate { jPlayerPrefs.Save(); });

        // COLOURBLIND ASSIST
        CB_Assists = new List<ColourblindAssistObject>();
        //does not cause sound at first frame bc we add sound after changing values
        colourblindToggle.isOn = GetColourblindAssist();
        colourblindToggle.onValueChanged.AddListener(delegate { uiSounds.Click(); });
        colourblindToggle.onValueChanged.AddListener(delegate { jPlayerPrefs.Save(); });

        // QUALITY SETTINGS
        qualityToggleGroup.SetAllTogglesOff();
        var toggleIndex = GetQualityLevel();

        Toggle[] groupChildren = qualityToggleGroup.transform.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < groupChildren.Length; i++)
        {
            if (i == toggleIndex)
                groupChildren[i].isOn = true;
            else
                groupChildren[i].isOn = false;

            //does not cause sound at first frame
            groupChildren[i].onValueChanged.AddListener(delegate { uiSounds.Click(); });
            groupChildren[i].onValueChanged.AddListener(delegate { jPlayerPrefs.Save(); });
        }
        
        // URL POP UP 
        confirmationPopUp = confirmationPopUpObject.GetComponent<ConfirmationPopUp>();
        keybindsList.SetActive(false);
        advancedSettings.SetActive(false);

        jPlayerPrefs.Save();

        // Hackerman Steam Achievement Check
        CheckForHackerman();
    }
    // HIDE COMPLETED STAGES SETTING
    public void SetHideCompletedStages(bool trueFalse)
    {
        jPlayerPrefs.SetInt("SETTINGS_STAGE_SELECT_HIDE_COMPLETED", trueFalse == true ? 1 : 0);
        jPlayerPrefs.Save();
    }
    public bool GetHideCompletedStages()
    {
        return jPlayerPrefs.GetInt("SETTINGS_STAGE_SELECT_HIDE_COMPLETED", 0) > 0; // return true if camera rotation is enabled, default to false
    }
    // HACKERMAN STEAM ACHIEVEMENT
    private void CheckForHackerman()
    {
        if (jPlayerPrefs.GetInt("HACKERMAN", 0) >= 1)
        {
            // unlock hackerman achievement
            MySteamAchievementHandler.SetAchievementCompleted(SteamAchNames.ACHIEVEMENT_HACKERMAN);
        }
    }
    // GAME SOUNDS
    public void ChangeVolumeMaster()
    {
        float input = Slider_Master.value;

        AudioListener.volume = input;
        jPlayerPrefs.SetFloat("SETTINGS_VOLUME_MASTER", input);
        jPlayerPrefs.Save();
    }
    public void ChangeVolumeMusic()
    {
        float input = Slider_Music.value;

        if (musicSource != null)
        {
            musicSource.volume = (input * 0.2f); // music is way too loud compared to sound effects
        }
        jPlayerPrefs.SetFloat("SETTINGS_VOLUME_MUSIC", input);
        jPlayerPrefs.Save();
    }
    // PLAYER CAMERA ROTATION
    public void TogglePlayerCamRotateWithPlayer()
    {
        jPlayerPrefs.SetInt("SETTINGS_CAMERA_ROTATEWITHPLAYER", camRotationToggle.isOn == true ? 1 : 0); // if the toggle is on after being pressed, set to 1
        jPlayerPrefs.Save();
    }
    public bool GetRotatePlayerCamWithPlayer()
    {
        return jPlayerPrefs.GetInt("SETTINGS_CAMERA_ROTATEWITHPLAYER", 0) > 0; // return true if camera rotation is enabled, default to false
    }
    // COLOURBLIND ASSISTANCE
    public void ToggleColourblindAssist()
    {
        jPlayerPrefs.SetInt("SETTINGS_ASSISTANCE_COLOURBLIND", colourblindToggle.isOn == true ? 1 : 0); // if the toggle is on after being pressed, set to 1
        foreach (ColourblindAssistObject cba in CB_Assists)
        {
            cba.RefreshThisObject();
        }
        jPlayerPrefs.Save();
    }
    public bool GetColourblindAssist()
    {
        return jPlayerPrefs.GetInt("SETTINGS_ASSISTANCE_COLOURBLIND", 1) > 0; // return true if colourblind assist is enabled
    }
    // AUTOMATIC CAMERA SWITCH ON PLAY
    public void ToggleCameraAutoSwitchOnStart()
    {
        jPlayerPrefs.SetInt("SETTINGS_CAMERA_AUTOMATIC_SWITCH_ON_START", camSwitchOnStartToggle.isOn == true ? 1 : 0); // if the toggle is on after being pressed, set to 1
        jPlayerPrefs.Save();
    }
    public bool GetCameraAutoSwitchOnStart()
    {
        return jPlayerPrefs.GetInt("SETTINGS_CAMERA_AUTOMATIC_SWITCH_ON_START", 1) > 0; // default to true
    }
    // AUTOMATIC MAX GAME SPEED
    public void ToggleAlwaysMaxGameSpeed()
    {
        jPlayerPrefs.SetInt("SETTINGS_GAMESPEED_MAX_ALWAYS", maxGameSpeedToggle.isOn == true ? 1 : 0); // if the toggle is on after being pressed, set to 1
        jPlayerPrefs.Save();
    }
    public bool GetAlwaysMaxGameSpeed()
    {
        return jPlayerPrefs.GetInt("SETTINGS_GAMESPEED_MAX_ALWAYS", 0) > 0; // default to true
    }
    // QUALITY LEVEL
    public void SetQualityLevel(int index)
    {
        var length = QualitySettings.names.Length;
        for (int i = 0; i < length; i++)
        {
            if (index == i) // only go inside if given a valid index
            {
                // CONTROL SHADOWS AND LIGHTING via unity automatically
                QualitySettings.SetQualityLevel(i);

                // SET MAXIMUM TEXTURE RESOLUTION
                // masterTexLimit is decreased to a quarter of the current res for each +1 above zero.
                // therefore, we input the inverse of i so that max res(index 5) gives 0 and lowest res(index 0) gives 5
                QualitySettings.masterTextureLimit = length - i;

                // SET ANTI-ALIASING
                // antiAliasing can be 0, 2 ,4 ,8
                float rangePercent = (float)i / (float)(length-1); // without the parentheses around (length-1) the division gets calculated first and rangePercent is negative every time
                QualitySettings.antiAliasing = (int)(rangePercent * 8f);
                // 0 = 0, 1 = 0, 2 = 2, 3 = 4, 4 = 4, 5 = 8
                
                // PARTICLES SOFT OR NOT
                if (i < 2) QualitySettings.softParticles = false; // if Low or MedLow settings, disable good particle blending
                else QualitySettings.softParticles = true; 

                // SAVE THE SETTINGS
                jPlayerPrefs.SetInt("SETTINGS_QUALITY_LEVEL", index);
            }
        }
    }

    public int GetQualityLevel()
    {
        return jPlayerPrefs.GetInt("SETTINGS_QUALITY_LEVEL", QualitySettings.names.Length - 1); // get saved option or default to highest
    }
    /// EXTRA CONFIRMATION BOX
    public void CloseConfirmationPopUp()
    {
        confirmationPopUpObject.SetActive(false);
    }
    /// DELETE ACHIEVEMENT CONFIRMATION
    public void OpenDeleteAllAchievementsConfirmation()
    {
        // removing previous actions from "Yes" button
        confirmationPopUp.confirmButton.onClick.RemoveAllListeners();
        // this specific confirmation
        confirmationPopUpObject.SetActive(true);
        confirmationPopUp.title.text = "Delete All Steam Achievements?";
        confirmationPopUp.description.text = "You are about to delete all achievements. This means all steam achievements will be as if you had never opened the game. Continue?";
        confirmationPopUp.confirmButton.onClick.AddListener(() => DoubleCheckDeleteAllAchievements());
    }
    private void DoubleCheckDeleteAllAchievements()
    {
        // removing previous actions from "Yes" button
        confirmationPopUp.confirmButton.onClick.RemoveAllListeners();
        // this specific confirmation
        confirmationPopUpObject.SetActive(true);
        confirmationPopUp.title.text = "Just Double Checking :)";
        confirmationPopUp.description.text = "This will delete all achievements. Are you sure you want to start over?";
        confirmationPopUp.confirmButton.onClick.AddListener(() => MySteamAchievementHandler.ResetSteamStats());
        confirmationPopUp.confirmButton.onClick.AddListener(() => AchievementsDeletedNotice());

        // switch yes/no button positions in case player double clicks on accident
        var tmpPos = confirmationPopUp.confirmButton.transform.position;
        confirmationPopUp.confirmButton.transform.position = confirmationPopUp.closeButton.transform.position;
        confirmationPopUp.closeButton.transform.position = tmpPos;
    }
    private void AchievementsDeletedNotice()
    {
        // removing previous actions from "Yes" button
        confirmationPopUp.confirmButton.onClick.RemoveAllListeners();
        // this specific confirmation
        confirmationPopUpObject.SetActive(true);
        confirmationPopUp.title.text = "Success";
        confirmationPopUp.description.text = "All Space Voyage Achievements have been deleted. Steam may take a second to realize.";
        confirmationPopUp.confirmButton.onClick.AddListener(() => confirmationPopUpObject.SetActive(false));

        // switch yes/no button positions in case player double clicks on accident
        var tmpPos = confirmationPopUp.confirmButton.transform.position;
        confirmationPopUp.confirmButton.transform.position = confirmationPopUp.closeButton.transform.position;
        confirmationPopUp.closeButton.transform.position = tmpPos;
    }
    /// DELETE PlayerPrefs CONFIRMATION
    public void OpenDeleteAllPlayerPrefsConfirmation()
    {
        // removing previous actions from "Yes" button
        confirmationPopUp.confirmButton.onClick.RemoveAllListeners();
        // this specific confirmation
        confirmationPopUpObject.SetActive(true);
        confirmationPopUp.title.text = "Delete All Game Data?";
        confirmationPopUp.description.text = "You are about to delete all saves. This means all records of completed stages and levels will be gone forever. Continue?";
        confirmationPopUp.confirmButton.onClick.AddListener(() => DoubleCheckDeleteAllGameSaves());
    }
    private void DoubleCheckDeleteAllGameSaves()
    {
        // removing previous actions from "Yes" button
        confirmationPopUp.confirmButton.onClick.RemoveAllListeners();
        // this specific confirmation
        confirmationPopUpObject.SetActive(true);
        confirmationPopUp.title.text = "Just Double Checking :)";
        confirmationPopUp.description.text = "This will delete all your game progress. Are you sure you want to start over?";
        confirmationPopUp.confirmButton.onClick.AddListener(() => DeleteAllGameSaves());

        // switch yes/no button positions in case player double clicks on accident
        var tmpPos = confirmationPopUp.confirmButton.transform.position;
        confirmationPopUp.confirmButton.transform.position = confirmationPopUp.closeButton.transform.position;
        confirmationPopUp.closeButton.transform.position = tmpPos;
    }
    public void DeleteAllGameSaves()
    {
        string[] sceneNames = SceneLoader.AskFor.SceneNames();
        for (int i = 0; i < sceneNames.Length; i++)
        {
            if (sceneNames[i] == "Tutorial") continue;

            var key = $"LEVEL_COMPLETE_{sceneNames[i]}";
            if (jPlayerPrefs.HasKey(key))
            {
                jPlayerPrefs.DeleteKey(key);
            }
        }

        jPlayerPrefs.Save();

        UIStateHandler.AskFor.GoToPreviousUIState();
        SceneLoader.AskFor.ReloadThisScene();
    }
    /// OPEN URL CONFIRMATION
    public void OpenURLConfirmation(string url)
    {
        // removing previous actions from "Yes" button
        confirmationPopUp.confirmButton.onClick.RemoveAllListeners();
        // this specific confirmation
        confirmationPopUpObject.SetActive(true);
        var slashIndex = url.LastIndexOf("www."); // find index of www.
        var title = url.Substring(slashIndex+4); // delete www. and everything before that
        if (title[title.Length-1] == '/') title = title.Substring(0, title.Length-1); // delete the last "/" at the end of the link
        confirmationPopUp.title.text = title;
        confirmationPopUp.description.text = "You are about to open a browser window. Continue?";
        confirmationPopUp.confirmButton.onClick.AddListener(() => OpenURL(url));
    }
    private void OpenURL(string url)
    {
        Application.OpenURL(url);
        CloseConfirmationPopUp();
    }
    // LOOK AT KEYBINDS
    public void OpenKeybinds()
    {
        keybindsList.SetActive(true);
    }
    public void CloseKeybinds()
    {
        keybindsList.SetActive(false);
    }
    // LOOK AT ADVANCED SETTINGS
    public void OpenAdvancedSettings()
    {
        advancedSettings.SetActive(true);
    }
    public void CloseAdvancedSettings()
    {
        advancedSettings.SetActive(false);
    }
    // TUTORIAL HAS BEEN PLAYED BEFORE CHECK
    public bool GetTutorialCompleted()
    {
        return jPlayerPrefs.GetInt("LEVEL_COMPLETE_Tutorial", 0) > 0; // returns 0 if tutorial has never been completed
    }

    // SAVE ALL
    public void SaveAll()
    {
        // is triggered before quit game
        jPlayerPrefs.Save();
    }

}
